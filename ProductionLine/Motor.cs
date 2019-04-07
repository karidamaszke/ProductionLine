using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLine
{
    class Motor
    {
        public int speed;
        public int temperature;

        public Motor()
        {
            this.speed = 50;
            this.temperature = 80;
        }

        public void SlowDown()
        {
            if (this.speed >= 5)
            {
                this.speed -= 5;
            }
            else
            {
                this.speed = 0;
            }

        }

        public void FastUp()
        {
            if (this.speed <= 95)
            {
                this.speed += 5;
            }
            else
            {
                this.speed = 100;
            }
        }

        public void RandomTemperature()
        {
            Random rand = new Random();
            this.temperature = rand.Next(10, 120);
        }

        public void SlowEngine()
        {
            this.speed = 10;
        }

        public void ChangeTemperature(int i)
        {
            if (this.temperature + i < 20)
            {
                this.temperature = 20;
            }
            else
            {
                this.temperature += i;
            }
        }
    }
}
