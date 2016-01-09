using CoveoBlitz;

namespace Coveo.State
{
    abstract class State
    {
        public int CostToTavern { get; set; }
        public int CostToMIne { get; set; }
        public int Life { get; set; }
        public int Gold { get; set; }

        private string Move()
        {
            var mtv = Direction.Stay;

            if (Life < CostToTavern + Constant.LifeDrainOnHit)
            {
                //Get closest tavern and move toward it
            }


            return mtv;
        }
    }
}
