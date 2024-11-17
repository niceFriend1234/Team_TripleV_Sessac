using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class SeasonsSystemURP : MonoBehaviour
{
    [SerializeField] private float transitionTime;

    [Header("Seasons post processing")]
    public Volume springPP;
    public Volume summerPP;
    public Volume autumnPP;
    public Volume winterPP;

    [Header("Seasons particle effects")]
    public GameObject springParticlesFolder;
    public GameObject summerParticlesFolder;
    public GameObject autumnParticlesFolder;
    public GameObject winterParticlesFolder;
    private ParticleSystem[] springParticles;
    private ParticleSystem[] summerParticles;
    private ParticleSystem[] autumnParticles;
    private ParticleSystem[] winterParticles;

    [Header("General setting")]
    public bool logToConsole;

    [Range(0.05f, 1f)]
    public float seasonSwitchSpeed;
    public seasons initialSeason;

    [Header("Fog")]
    public float springFogDensity;
    public float summerFogDensity;
    public float autumnFogDensity;
    public float winterFogDensity;

    [Range(0.01f, 0.5f)]
    public float fogDensitySwitchSpeed;

    private float seasonFogDensity;

    [Header("Grass")]
    public bool changeGrassColor;
    public bool changeGrassWindSpeed;
    [Space(1f)]
    public float grassSwitchSpeed;
    public Terrain terrain;
    public GrassColor[] grassColors;
    private DetailPrototype[] detailPrototypes;
    public WindSpeed windSpeed;

    [Header("Materials")]
    public SeasonMaterial[] seasonMaterials;

    public static int dayOfMonth = 1;
    public static int days_total = 1;

    public static seasons season { get; private set; }

    private Dictionary<seasons, int> seasonDays = new Dictionary<seasons, int>();

    private bool isNewSeasonPostProcessingSet;
    private bool isGrassColorsSet;
    private bool isAllMaterialsRecolored;
    public static Action<seasons> onSeasonChange;
    private void OnEnable()
    {
        onSeasonChange += SetSeason;
    }

    private void OnDisable()
    {
        onSeasonChange -= SetSeason;
    }

    void Start()
    {
        season = initialSeason;
        onNewSeasonActions();
        if (changeGrassColor || changeGrassWindSpeed) { detailPrototypes = terrain.terrainData.detailPrototypes; }
        findParticlesInFolders();
    }


    void Update()
    {
        seasonPostProcessing();
        checkAndSetFogDensity();
        recolorMaterials();
        if (changeGrassColor) { recolorGrass(); }
        seasonEffects();
    }


    public async void SetSeason(seasons season)
    {
        SeasonsSystemURP.season = season;
        onNewSeasonActions();

        await RunOnMainThread(async () =>
        {
            float elapsed = 0f;
            float initialSpringWeight = springPP.weight;
            float initialSummerWeight = summerPP.weight;
            float initialAutumnWeight = autumnPP.weight;
            float initialWinterWeight = winterPP.weight;
            playParticleEffects(springParticles, false);
            playParticleEffects(summerParticles, false);
            playParticleEffects(autumnParticles, false);
            playParticleEffects(winterParticles, false);
            playParticleEffects(GetSeasonParticles(season), true);
            while (elapsed < transitionTime)
            {
                float t = elapsed / transitionTime;
                springPP.weight = Mathf.Lerp(initialSpringWeight, 0, t);
                summerPP.weight = Mathf.Lerp(initialSummerWeight, 0, t);
                autumnPP.weight = Mathf.Lerp(initialAutumnWeight, 0, t);
                winterPP.weight = Mathf.Lerp(initialWinterWeight, 0, t);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            springPP.weight = 0;
            summerPP.weight = 0;
            autumnPP.weight = 0;
            winterPP.weight = 0;
        });
        await RunOnMainThread(async () =>
        {
            for (int i = 0; i < grassColors.Length; i++)
            {
                detailPrototypes[i].dryColor = getSeasonalGrassColor(grassColors[i], false);
                detailPrototypes[i].healthyColor = getSeasonalGrassColor(grassColors[i], true);
            }
            RenderSettings.fogDensity = seasonFogDensity;
            terrain.terrainData.detailPrototypes = detailPrototypes;
            float elapsed = 0f;
            var postProcessing = GetSeasonPostProcessing(season);
            float initialWeight = postProcessing.weight;
            while (elapsed < transitionTime)
            {
                float t = elapsed / transitionTime;
                postProcessing.weight = Mathf.Lerp(initialWeight, 1, t);

                elapsed += Time.deltaTime;
                await Task.Yield(); // Stay on the main thread
            }

            postProcessing.weight = 1;
        });
        // Fog
    }
    private async Task RunOnMainThread(Func<Task> action)
    {
        if (UnityEngine.Application.isPlaying)
        {
            await action();
        }
        else
        {
            Debug.LogError("RunOnMainThread called outside play mode!");
        }
    }
    public void SetSeasonInstanly(seasons season)
    {
        SeasonsSystemURP.season = season;
        onNewSeasonActions();

        // Post processing
        springPP.weight = 0;
        summerPP.weight = 0;
        autumnPP.weight = 0;
        winterPP.weight = 0;
        var postProcessing = GetSeasonPostProcessing(season);
        postProcessing.weight = 1;

        // Particles
        playParticleEffects(springParticles, false);
        playParticleEffects(summerParticles, false);
        playParticleEffects(autumnParticles, false);
        playParticleEffects(winterParticles, false);
        playParticleEffects(GetSeasonParticles(season), true);

        // Fog
        RenderSettings.fogDensity = seasonFogDensity;

        // Grass color
        for (int i = 0; i < grassColors.Length; i++)
        {
            detailPrototypes[i].dryColor = getSeasonalGrassColor(grassColors[i], false);
            detailPrototypes[i].healthyColor = getSeasonalGrassColor(grassColors[i], true);
        }
        terrain.terrainData.detailPrototypes = detailPrototypes;
    }

    private void findParticlesInFolders()
    {
        springParticles = springParticlesFolder.GetComponentsInChildren<ParticleSystem>();
        summerParticles = summerParticlesFolder.GetComponentsInChildren<ParticleSystem>();
        autumnParticles = autumnParticlesFolder.GetComponentsInChildren<ParticleSystem>();
        winterParticles = winterParticlesFolder.GetComponentsInChildren<ParticleSystem>();
    }

    private void onNewSeasonActions()
    {
        isNewSeasonPostProcessingSet = false;
        isGrassColorsSet = false;
        isAllMaterialsRecolored = false;

        if (changeGrassWindSpeed) { changeGrassSpeed(); }

        switch (season)
        {
            case seasons.spring:
                seasonFogDensity = springFogDensity;
                break;
            case seasons.summer:
                seasonFogDensity = summerFogDensity;
                break;
            case seasons.autumn:
                seasonFogDensity = autumnFogDensity;
                break;
            case seasons.winter:
                seasonFogDensity = winterFogDensity;
                break;
        }
    }

    private void seasonEffects()
    {
        switch (season)
        {
            case seasons.spring:
                winterEffects(false);
                playParticleEffects(springParticles, true);
                break;
            case seasons.summer:
                playParticleEffects(springParticles, false);
                playParticleEffects(summerParticles, true);
                break;
            case seasons.autumn:
                playParticleEffects(summerParticles, false);
                playParticleEffects(autumnParticles, true);
                break;
            case seasons.winter:
                autumnEffects(false);
                winterEffects(true);
                break;
            default:
                break;
        }
    }

    private ParticleSystem[] GetSeasonParticles(seasons season)
    {
        switch (season)
        {
            case seasons.spring:
                return springParticles;
            case seasons.summer:
                return summerParticles;
            case seasons.autumn:
                return autumnParticles;
            case seasons.winter:
                return winterParticles;
        }

        return new[] { new ParticleSystem() };
    }

    private void playParticleEffects(ParticleSystem[] effects, bool play)
    {
        if (effects == null) { return; }

        foreach (var effect in effects)
        {
            if (play) { effect.Play(); }
            else { effect.Stop(); }
        }
    }

    private void autumnEffects(bool play)
    {
        if (autumnParticles == null) { return; }

        foreach (var autumnParticle in autumnParticles)
        {
            if (play) { autumnParticle.Play(); }
            else { autumnParticle.Stop(); }
        }
    }

    private void winterEffects(bool play)
    {
        if (winterParticles == null) { return; }

        foreach (var winterParticle in winterParticles)
        {
            if (play) { winterParticle.Play(); }
            else { winterParticle.Stop(); }
        }
    }

    private void seasonPostProcessing()
    {
        if (isNewSeasonPostProcessingSet) { return; }

        switch (season)
        {
            case seasons.spring:
                isNewSeasonPostProcessingSet = setPostProcessing(false, winterPP);
                isNewSeasonPostProcessingSet = setPostProcessing(true, springPP);
                break;
            case seasons.summer:
                isNewSeasonPostProcessingSet = setPostProcessing(false, springPP);
                isNewSeasonPostProcessingSet = setPostProcessing(true, summerPP);
                break;
            case seasons.autumn:
                isNewSeasonPostProcessingSet = setPostProcessing(false, summerPP);
                isNewSeasonPostProcessingSet = setPostProcessing(true, autumnPP);
                break;
            case seasons.winter:
                isNewSeasonPostProcessingSet = setPostProcessing(false, autumnPP);
                isNewSeasonPostProcessingSet = setPostProcessing(true, winterPP);
                break;
            default:
                break;
        }
    }

    private Volume GetSeasonPostProcessing(seasons season)
    {
        switch (season)
        {
            case seasons.spring:
                return springPP;
            case seasons.summer:
                return summerPP;
            case seasons.autumn:
                return autumnPP;
            case seasons.winter:
                return winterPP;
        }

        return new Volume();
    }

    private bool setPostProcessing(bool set, Volume postProcessing)
    {
        if (postProcessing == null) { Debug.LogError(season.ToString() + " or previous season post processing is not set!"); return true; }

        if (set)
        {
            if (postProcessing.weight < 1) { postProcessing.weight += Time.deltaTime * seasonSwitchSpeed; return false; }
            else { postProcessing.weight = 1; return true; }
        }
        else
        {
            if (postProcessing.weight > 0.1) { postProcessing.weight -= Time.deltaTime * seasonSwitchSpeed; return false; }
            else { postProcessing.weight = 0; return true; }
        }
    }

    private void checkAndSetFogDensity()
    {
        if (RenderSettings.fogDensity < seasonFogDensity)
        {
            RenderSettings.fogDensity += (Time.deltaTime * fogDensitySwitchSpeed) / 1000;
        }

        if (RenderSettings.fogDensity > seasonFogDensity)
        {
            RenderSettings.fogDensity -= (Time.deltaTime * fogDensitySwitchSpeed) / 1000;
        }
    }


    private void recolorGrass()
    {
        if (isGrassColorsSet) { return; }

        bool isDryRecolorFinished = false;
        bool isHealthyRecolorFinished = false;
        Color lerpedColor;
        for (int i = 0; i < grassColors.Length; i++)
        {
            int grassPositionInTerrainData = grassColors[i].terrainPosition;
            (lerpedColor, isDryRecolorFinished) = lerpMaterialColors(detailPrototypes[grassPositionInTerrainData].dryColor, getSeasonalGrassColor(grassColors[i], false));
            if (!isDryRecolorFinished)
            {
                detailPrototypes[i].dryColor = lerpedColor;
            }

            (lerpedColor, isHealthyRecolorFinished) = lerpMaterialColors(detailPrototypes[grassPositionInTerrainData].healthyColor, getSeasonalGrassColor(grassColors[i], true));
            if (!isHealthyRecolorFinished)
            {
                detailPrototypes[i].healthyColor = lerpedColor;
            }
        }

        terrain.terrainData.detailPrototypes = detailPrototypes;

        if (isHealthyRecolorFinished && isDryRecolorFinished) { isGrassColorsSet = true; }
    }

    private void changeGrassSpeed()
    {
        switch (season)
        {
            case seasons.spring:
                terrain.terrainData.wavingGrassSpeed = windSpeed.spring;
                terrain.terrainData.wavingGrassStrength = windSpeed.spring;
                break;
            case seasons.summer:
                terrain.terrainData.wavingGrassSpeed = windSpeed.summer;
                terrain.terrainData.wavingGrassStrength = windSpeed.summer;
                break;
            case seasons.autumn:
                terrain.terrainData.wavingGrassSpeed = windSpeed.autumn;
                terrain.terrainData.wavingGrassStrength = windSpeed.autumn;
                break;
            case seasons.winter:
                terrain.terrainData.wavingGrassSpeed = windSpeed.winter;
                terrain.terrainData.wavingGrassStrength = windSpeed.winter;
                break;
        }
    }

    private Color getSeasonalGrassColor(GrassColor grassColor, bool healthy)
    {
        switch (season)
        {
            case seasons.spring:
                return healthy ? grassColor.springHealtyColor : grassColor.springDryColor;
            case seasons.summer:
                return healthy ? grassColor.summerHealtyColor : grassColor.summerDryColor;
            case seasons.autumn:
                return healthy ? grassColor.autumnHealtyColor : grassColor.autumnDryColor;
            case seasons.winter:
                return healthy ? grassColor.winterHealtyColor : grassColor.winterDryColor;
            default:
                return new Color();
        }
    }
    private void recolorMaterials()
    {
        if (seasonMaterials == null) { return; }

        if (isAllMaterialsRecolored) { return; }

        bool isMaterialRecolored;
        Color lerpedColor;
        for (int i = 0; i < seasonMaterials.Length; i++)
        {
            (lerpedColor, isMaterialRecolored) = lerpMaterialColors(seasonMaterials[i].getColor(), seasonMaterials[i].getSeasonColor());
            seasonMaterials[i].setColor(lerpedColor);
        }
    }
    private float checkValueOverflow(float value, float maxValue)
    {
        if (value > maxValue) { value = maxValue; }

        return value;
    }

    private (Color, bool) lerpMaterialColors(Color previousColor, Color newColor)
    {
        Color lerpedColor = Color.Lerp(previousColor, newColor, Time.deltaTime * grassSwitchSpeed);

        float r = lerpedColor.r - newColor.r;
        r = r < 0 ? r * -1 : r;
        float g = lerpedColor.g - newColor.g;
        g = g < 0 ? g * -1 : g;
        float b = lerpedColor.b - newColor.b;
        b = b < 0 ? b * -1 : b;

        float colorSimilarity = r + g + b;

        return (lerpedColor, colorSimilarity < 0.1f);
    }


    [System.Serializable]
    public class GrassColor
    {
        public int terrainPosition;

        public Color springHealtyColor;
        public Color springDryColor;
        public Color summerHealtyColor;
        public Color summerDryColor;
        public Color autumnHealtyColor;
        public Color autumnDryColor;
        public Color winterHealtyColor;
        public Color winterDryColor;
    }

    [System.Serializable]
    public class WindSpeed
    {
        public float spring;
        public float summer;
        public float autumn;
        public float winter;
    }

    [System.Serializable]
    public class SeasonMaterial
    {
        public Material material;
        public string colorPropertyName;

        public Color springColor;
        public Color summerColor;
        public Color autumnColor;
        public Color winterColor;

        public Color getSeasonColor()
        {
            switch (season)
            {
                case seasons.spring:
                    return springColor;
                case seasons.summer:
                    return summerColor;
                case seasons.autumn:
                    return autumnColor;
                case seasons.winter:
                    return winterColor;
                default:
                    return new Color();
            }
        }

        public Color getPreviousSeasonColor()
        {
            switch (season)
            {
                case seasons.spring:
                    return winterColor;
                case seasons.summer:
                    return springColor;
                case seasons.autumn:
                    return summerColor;
                case seasons.winter:
                    return autumnColor;
                default:
                    return new Color();
            }
        }

        public void setColor(Color color)
        {
            material.SetColor(colorPropertyName, color);
        }

        public Color getColor()
        {
            return material.GetColor(colorPropertyName);
        }
    }
    public enum seasons
    {
        none,
        spring,
        summer,
        autumn,
        winter
    }
}
