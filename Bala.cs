using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naves_Invasoras_2
{
    public class Bala : ObjetoGrafico
    {
        public Bala(string[] forma, ConsoleColor color, Tamaño tamaño, Posicion posicion)
            : base(forma, color, tamaño, posicion)
        {
        }

        public void Mover()
        {
            Borrar();
            Posicion.Decrementar_Y(); 
            Pintar();
        }

        public bool VerificarImpacto()
        {
            // Aquí va la lógica para verificar si chocó con una nave
            return false; // cambiar según implementación real
        }
    }
}
