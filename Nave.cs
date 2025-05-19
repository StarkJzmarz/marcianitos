// Nave.cs
using System;

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
            // Ya no borramos ni pintamos aquí. Solo actualizamos la posición lógica.
            // BorrarDeBuffer(); // Opcional si no limpias el buffer completo cada frame

            if (direccion == Direccion.Derecha)
            {
                // Agregar lógica de límites si es necesario, aunque el buffer lo maneja
                if (Posicion.X + Tamaño.Ancho < Console.WindowWidth - 1) // Ejemplo de límite
                    Posicion.Incrementa_X();
            }
            else if (direccion == Direccion.Izquierda)
            {
                if (Posicion.X > 0) // Ejemplo de límite
                    Posicion.Decrementa_X();
            }

            // PintarEnBuffer(); // Esto se hará en el bucle de renderizado principal
        }
    }
}