using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naves_Invasoras_2
{
    public class Nave : ObjetoGrafico
    {
        public Nave(string[] forma, ConsoleColor color, Tamaño tamaño, Posicion posicion)
            : base(forma, color, tamaño, posicion)
        {
        }

        public void Mover(Direccion direccion)
        {
            Borrar();

            if (direccion == Direccion.Derecha)
            {
                Posicion.Incrementa_X();
            }
            else if (direccion == Direccion.Izquierda)
            {
                Posicion.Decrementa_X();
            }

            Pintar();
        }
    }
}
