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
        public bool EstaActiva { get; set; }

        // Define la forma y tamaño base estáticos
        public static string[] FormaBase = {
                "  ▀▄    ▄▀ ",
                " ▄█▀███▀█▄ ",
                "█▀███████▀█",
                "█ █▀▀▀▀▀█ █",
                "   ▀▀ ▀▀   ",
    };

        public static ConsoleColor ColorBase = ConsoleColor.Red;

        public static ConsoleColor ColorBaseOriginal = ConsoleColor.Red; // Color original
        // Podríamos cambiar el color al recibir daño
        public static ConsoleColor ColorDañoLeve = ConsoleColor.DarkYellow;
        public static ConsoleColor ColorDañoMedio = ConsoleColor.DarkRed;


        public static Tamaño TamañoBase = new Tamaño(FormaBase.Length, FormaBase[0].Length);

        public NaveInvasora(Posicion posicion, int fila)
            : base(
                  FormaBase,
                ColorBaseOriginal,
                TamañoBase,
                posicion
            )
        {
            this.Fila = fila;
            this.EstaActiva = true;
            //Escudo = fila switch { 1 => 1, 2 => 2, 3 => 3, _ => 1 };


            // Asignar escudo basado en la fila
            // La expresión switch ya hace esto, la duplicación no es necesaria.
            // Escudo = fila switch { 1 => 1, 2 => 2, 3 => 3, _ => 1 }; // Fila 1 es la más baja/cercana

            // Corregido para que fila 1 (más baja) tenga menos escudo
            // y fila 3 (más alta) tenga más.
            // Si tus filas se numeran de arriba (0, 1, 2) hacia abajo:
            // Fila 0 (superior) -> 3 escudos
            // Fila 1 (media)    -> 2 escudos
            // Fila 2 (inferior) -> 1 escudo
            // Ajusta esto según cómo interpretes 'fila'. Asumiré que 'fila' viene de tu bucle
            // de creación, donde fila 0 es la superior.

            switch (fila) // 'fila' recibida (0, 1, 2 para 3 filas)
            {
                case 0: // Fila superior
                    Escudo = 4;
                    this.Color = ColorBaseOriginal; // Podrías tener un color diferente para naves más fuertes
                    break;
                case 1: // Fila media
                    Escudo = 3;
                    this.Color = ColorBaseOriginal;
                    break;
                case 2: // Fila inferior
                    Escudo = 2;
                    this.Color = ColorBaseOriginal;
                    break;
                default: // Por si acaso
                    Escudo = 1;
                    this.Color = ColorBaseOriginal;
                    break;
            }
        }
        public bool RecibirImpacto()
        {
            if (!EstaActiva) return false; // Ya está destruida

            Escudo--;

            if (Escudo <= 0)
            {
                EstaActiva = false; // Marcar como destruida
                // Opcional: Podrías cambiar la forma a una explosión aquí
                // this.Forma = FormaExplosion;
                // this.Color = ColorExplosion;
                return true; // Destruida
            }
            else
            {
                // Opcional: Cambiar color para indicar daño
                if ((Escudo == 2 || Escudo ==3)) // Si era una nave de 3 escudos y ahora tiene 2
                {
                    this.Color = ColorDañoLeve;
                }
                else if ((Escudo == 1 || Escudo == 0 )) // Si era de 3 o 2 y ahora tiene 1
                {
                    this.Color = ColorDañoMedio;
                }
                return false; // No destruida, solo dañada
            }
        }
    }
}


