using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLine
{
    class Processor
    {
        public int temperature;
        public int utilization;

        public Processor()
        {
            this.temperature = 60;
            this.utilization = 20;
        }

        public void RandomTemperature()
        {
            Random rand = new Random();
            this.temperature = rand.Next(10, 120);
        }

        public void ChangeTemperature(int i)
        {
            if (this.temperature + i < 20)
                this.temperature = 20;
            else
                this.temperature += i;            
        }

        public void ChangeUtilization(int i)
        {
            if (this.utilization + i < 1)
                this.utilization = 1;
            else
                this.utilization += i;
        }
    }
}
