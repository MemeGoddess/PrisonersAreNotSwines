using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonersAreNotSwines
{
    public class JobGiver_CleanCell : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Room cell = pawn.GetRoom();
            if (cell == null || !cell.IsPrisonCell)
            {
                return null;
            }

            Map map = pawn.Map;
            Job job = JobMaker.MakeJob(JobDefOf.Clean);
            foreach (IntVec3 tile in cell.Cells)
            {
                List<Thing> thingList = tile.GetThingList(map);
                foreach (Thing t in thingList)
                {
                    if (HasJobOnThing(pawn, t))
                    {
                        job.AddQueuedTarget(TargetIndex.A, t);
                    }
                }
                if (job.GetTargetQueue(TargetIndex.A).Count >= 15)
                {
                    break;
                }
            }
            if (job.targetQueueA == null || job.targetQueueA.Count < 1)
            {
                return null;
            }
            job.targetQueueA.SortBy(targ => targ.Cell.DistanceToSquared(pawn.Position));
            return job;
        }
        private bool HasJobOnThing(Pawn pawn, Thing t)
        {
            if (!(t is Filth filth))
                return false;

            if (!filth.Map.areaManager.Home[filth.Position])
                return false;

            if (!pawn.CanReserve(t, 1, -1, null, false))
                return false;

            if (filth.TicksSinceThickened < 600)
                return false;

            return true;
        }
    }
}
