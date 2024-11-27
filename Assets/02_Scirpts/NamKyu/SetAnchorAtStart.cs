// using UnityEngine;
// using UnityEngine.XR.ARFoundation;

// public class SetAnchorAtStart : MonoBehaviour
// {
//     private ARAnchorManager anchorManager;
//     private ARAnchor anchor;

//     void Start()
//     {
//         // ARAnchorManager 컴포넌트 찾기
//         anchorManager = FindObjectOfType<ARAnchorManager>();

//         if (anchorManager != null)
//         {
//             // 현재 오브젝트의 위치와 회전으로 앵커 생성
//             anchor = anchorManager.AddAnchor(new Pose(transform.position, transform.rotation));

//             if (anchor != null)
//             {
//                 Debug.Log("Anchor successfully created and attached.");
//             }
//             else
//             {
//                 Debug.LogWarning("Failed to create anchor.");
//             }
//         }
//         else
//         {
//             Debug.LogWarning("ARAnchorManager is not found in the scene.");
//         }
//     }
// }