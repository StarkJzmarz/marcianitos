using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naves_Invasoras_2
{
    public class Jugador
    {
        private string Nombre;
        public bool PuedeDisparar { get; set; } = true;
        public NaveDefensora Nave { get; private set; }
        private int Puntaje;

        public Posicion posicion { get; set; }

        public Jugador(string nombre, Posicion posicion, NaveDefensora nave)
        {
            this.Nombre = nombre;
            this.Nave = nave;
            this.Puntaje = 0;
            this.posicion = posicion;
        }

        public int SumarPuntos(int valor)
        {
            Puntaje += valor;
            return Puntaje;
        }

        public string GetNombre()
        {
            return Nombre;
        }

        public int GetPuntaje()
        {
            return Puntaje;
        }

        internal void IncrementarPuntaje(int v)
        {
            this.Puntaje += v;
        }
    }
}
