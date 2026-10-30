using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageMaterials.wpf
{
    public static class ProductCalculator
    {    
        
            public static int CalculateProductQuantity(
                int productTypeId,
                int materialTypeId,
                int rawQuantity,
                double param1,
                double param2)
            {
                
                if (productTypeId <= 0 || materialTypeId <= 0 || rawQuantity <= 0 ||
                    param1 <= 0 || param2 <= 0)
                {
                    return -1;
                }

            double productCoefficient;
            switch (productTypeId)
            {
                case 1: productCoefficient = 1.0; break;
                case 2: productCoefficient = 1.2; break;
                case 3: productCoefficient = 0.9; break;
                default: return -1;
            }

            double lossPercent;
            switch (materialTypeId)
            {
                case 1: lossPercent = 5.0; break;
                case 2: lossPercent = 3.0; break;
                case 3: lossPercent = 2.0; break;
                case 4: lossPercent = 1.0; break;
                case 5: lossPercent = 4.0; break;
                default: return -1;
            }

            if (lossPercent < 0) return -1;

                double rawPerUnit = param1 * param2 * productCoefficient;

                if (rawPerUnit <= 0) return -1;

                double availableRaw = rawQuantity * (1.0 - lossPercent / 100.0);

                int result = (int)Math.Floor(availableRaw / rawPerUnit);

                return result >= 0 ? result : -1;
            }
        }

    
}
