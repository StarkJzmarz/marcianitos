using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naves_Invasoras_2
{
    public class Posicion
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Posicion(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public void Incrementa_X() { X++; }

        public void Decrementar_Y() => Y++;

        public void Incrementar_Y()  { if (Y > 0) Y--; }

        public void Decrementa_X() { if (X > 0) X--; }
 

    }
}
