using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naves_Invasoras_2
{
    public class NaveInvasora : Nave
    {
        public int Fila { get; private set; }
        public int Escudo { get; private set; }
        public bool EstaActiva { get; private set; }

        public NaveInvasora(Posicion posicion, int fila)
            : base(
                new string[]
                {
                "    ▀▄    ▄▀   ",
                "   ▄█▀███▀█▄   ",
                "  █▀███████▀█  ",
                "  █ █▀▀▀▀▀█ █  ",
                "     ▀▀ ▀▀     ", 
                },
                ConsoleColor.Red,
                new Tamaño(15, 5),  
                posicion
            )
        {
            this.Fila = fila;
            this.EstaActiva = true;
            Escudo = fila switch { 1 => 1, 2 => 2, 3 => 3, _ => 1 };


            switch (fila)
            {
                case 1: Escudo = 1; break;
                case 2: Escudo = 2; break;
                case 3: Escudo = 3; break;
                default: Escudo = 1; break;
            }
        }

        public void RecibirImpacto()
        {
            if (!EstaActiva) return;
            Escudo = QuitarEscudo(1);
            if (DestruirNave(Escudo))
            {
                Borrar();
                EstaActiva = false;
            }
        }

        public int QuitarEscudo(int impacto)
        {
            return Math.Max(Escudo - impacto, 0);
        }

        public bool DestruirNave(int escudo)
        {
            return escudo <= 0;
        }
    }
}
