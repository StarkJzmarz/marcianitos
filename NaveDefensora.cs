using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naves_Invasoras_2
{
    public class NaveDefensora : Nave
    {

        // Define la forma y tamaño base estáticos
        public static string[] FormaBase = {
                "        █        ",
                "       ███       ",
                "    █  █ █  █    ",
                " █  ████ ████  █ ",
                " ███████████████ ",
                " ████  ███  ████ ",
                " █  █   █   █  █ ",
                " █             █ ",
    };

        public static ConsoleColor ColorBase = ConsoleColor.Cyan;

        public static Tamaño TamañoBase = new Tamaño(FormaBase[0].Length, FormaBase.Length);

        private long ultimoDisparoTicks;
        private const int COOLDOWN_DISPARO_MS = 300;
        public Bala bala;

        public NaveDefensora(Posicion posicion)
        : base(
              FormaBase,
            ColorBase,
            TamañoBase,
            posicion)
        {
            ultimoDisparoTicks = 0;
            this.bala = null; // Inicialmente sin bala
        }
        public Bala IntentarDisparar(Jugador jugador)
        {

            // Solo disparar si no hay una bala activa (MiBala es null o MiBala.Activa es false)
            if (bala == null || !bala.Activa)
            {
                long TicksAhora = Environment.TickCount64;
                if (TicksAhora - ultimoDisparoTicks > COOLDOWN_DISPARO_MS)
                {

                    ultimoDisparoTicks = TicksAhora;
                    // Calcular la posición inicial de la bala
                    int posXDisparo = this.Posicion.X + (this.Tamaño.Ancho);
           

                    int posYDisparo = this.Posicion.Y - 1; // Justo encima de la nave


                    if (posYDisparo >= 0)
                    {
                        // Aquí asumo que Direccion.Arriba es un valor de tu enum Direccion
                        this.bala = new Bala(new Posicion(posXDisparo, posYDisparo), Direccion.Arriba, jugador);
                        return this.bala;
                    }
                }

            }

            return null; // No se pudo disparar (cooldown) o posición inválida
        }
    }
}
