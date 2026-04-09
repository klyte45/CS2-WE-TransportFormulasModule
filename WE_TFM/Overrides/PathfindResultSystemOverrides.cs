using Belzont.Utils;
using Colossal.Entities;
using Game.Pathfind;
using Game.Routes;
using Unity.Entities;
using Unity.Jobs;
using WE_TFM.Components;
using WE_TFM.Components.Shareable;

namespace WE_TFM.Overrides
{
    public class PathfindResultSystemOverrides : Redirector, IRedirectable
    {
        private EntityManager entityManager;
        private static PathfindResultSystemOverrides Instance;

        public void DoPatches(World world)
        {
            Instance = this;
            entityManager = world.EntityManager;
            AddRedirect(
                 typeof(PathfindResultSystem).GetMethod("ProcessResults", RedirectorUtils.allFlags, null, [typeof(PathfindQueueSystem.ActionList<PathfindAction>), typeof(JobHandle).MakeByRefType(), typeof(JobHandle)], null),
                 GetType().GetMethod(nameof(BeforeProcessResults), RedirectorUtils.allFlags));
        }

        private static void BeforeProcessResults(PathfindQueueSystem.ActionList<PathfindAction> list)
        {
            var em = Instance.entityManager;
            for (int i = 0; i < list.m_Items.Count; i++)
            {
                var action = list.m_Items[i];
                if ((action.m_Flags & PathFlags.Scheduled) != 0
                    && action.m_Action.data.m_State == PathfindActionState.Completed
                    && em.HasComponent<CurrentRoute>(action.m_Owner)
                    && em.TryGetComponent<PathInformation>(action.m_Owner, out var pathInformation))
                {
                    em.AddComponentData(action.m_Owner, new WE_TFM_DirtyVehicle
                    {
                        oldTarget = pathInformation.m_Destination,
                    });
                }
            }
        }
    }
}