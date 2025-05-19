using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naves_Invasoras_2
{
    public class Bala : ObjetoGrafico
    {
        public bool Activa { get; set; }
        private Direccion direccionMovimiento; // Ya tienes tu clase Direccion, ¡genial!

        public static string[] FormaBaseBalaDefensora = { "|" };
        public static ConsoleColor ColorBaseBalaDefensora = ConsoleColor.Yellow;
        public static Tamaño TamañoBaseBalaDefensora = new Tamaño(FormaBaseBalaDefensora[0].Length, FormaBaseBalaDefensora.Length);
        
        public Jugador Propietario { get; private set; } // Jugador que disparó esta bala

        public Bala(Posicion posicionInicial, Direccion direccion /*, OrigenBala origen*/, Jugador propietario)
            : base(FormaBaseBalaDefensora, ColorBaseBalaDefensora, TamañoBaseBalaDefensora, posicionInicial)
        {
            this.Activa = true;
            this.direccionMovimiento = direccion;
            this.Propietario = propietario;
        }

        public void Mover()
        {
            if (!Activa) return;

            Posicion.Incrementar_Y();
            
            if (Posicion.Y <= 0 || Posicion.Y >= Console.WindowHeight) // O usa bufferAlto
            {
                Activa = false;
            }
        }
    }
}
