// ObjetoGrafico.cs
using System;

namespace Naves_Invasoras_2
{
    public class ObjetoGrafico
    {
        protected string[] Forma;
        public Posicion Posicion { get; set; }
        public Tamaño Tamaño { get; set; }
        protected ConsoleColor Color;

        // Buffer para dibujar. Podrías usar una estructura más compleja
        // si necesitas colores por caracter, pero empecemos simple.
        public static char[,] screenBuffer;
        public static ConsoleColor[,] colorBuffer; // Para los colores

        public ObjetoGrafico(string[] forma, ConsoleColor color, Tamaño tamaño, Posicion posicion)
        {
            this.Forma = forma;
            this.Color = color; // <--- Asegúrate que el 'color' recibido se asigna a 'this.Color'
            this.Tamaño = tamaño;
            this.Posicion = posicion;
        }

        public void setColor(ConsoleColor color)
        {
            this.Color = color;
        }

        // Método estático para inicializar el buffer desde la clase Juego
        public static void InicializarBuffer(int ancho, int alto)
        {
            screenBuffer = new char[alto, ancho];
            colorBuffer = new ConsoleColor[alto, ancho];
            LimpiarBuffer();
        }

        public static void LimpiarBuffer()
        {
            if (screenBuffer == null) return;
            for (int y = 0; y < screenBuffer.GetLength(0); y++)
            {
                for (int x = 0; x < screenBuffer.GetLength(1); x++)
                {
                    screenBuffer[y, x] = ' ';
                    colorBuffer[y, x] = Console.BackgroundColor; // O un color por defecto
                }
            }
        }

        // Pintar en el buffer
        public virtual void PintarEnBuffer()
        {
            if (screenBuffer == null) return; // Buffer no inicializado

            for (int i = 0; i < Forma.Length; i++)
            {
                for (int j = 0; j < Forma[i].Length; j++)
                {
                    int bufferX = Posicion.X + j;
                    int bufferY = Posicion.Y + i;

                    if (Forma[i][j] != ' ') // Solo pinta caracteres no espaciales con el color del objeto
                    {
                        // Comprobar límites del buffer
                        if (bufferY >= 0 && bufferY < screenBuffer.GetLength(0) &&
                        bufferX >= 0 && bufferX < screenBuffer.GetLength(1))
                        {
                            screenBuffer[bufferY, bufferX] = Forma[i][j];
                            colorBuffer[bufferY, bufferX] = this.Color;
                        }
                    }
                }
            }
        }

        // El Borrar individual ya no es tan necesario si limpiamos el buffer cada frame.
        // Si lo necesitas para optimizaciones (borrar solo un objeto específico sin limpiar todo),
        // lo implementarías de forma similar a PintarEnBuffer pero escribiendo ' '.
        public virtual void BorrarDeBuffer()
        {
            if (screenBuffer == null) return;

            for (int i = 0; i < Forma.Length; i++)
            {
                for (int j = 0; j < (Forma[i]?.Length ?? 0); j++) // Asegurarse que Forma[i] no es null
                {
                    int bufferX = Posicion.X + j;
                    int bufferY = Posicion.Y + i;

                    if (bufferY >= 0 && bufferY < screenBuffer.GetLength(0) &&
                        bufferX >= 0 && bufferX < screenBuffer.GetLength(1))
                    {
                        screenBuffer[bufferY, bufferX] = ' ';
                        // Podrías resetear el color también si es necesario
                        // colorBuffer[bufferY, bufferX] = Console.BackgroundColor;
                    }
                }
            }
        }


        // Estos métodos ya no se usarán directamente para el juego principal,
        // pero pueden ser útiles para debug o elementos fuera del buffer.
        public virtual void PintarDirecto()
        {
            Console.ForegroundColor = Color;
            for (int i = 0; i < Forma.Length; i++)
            {
                int x = Posicion.X;
                int y = Posicion.Y + i;

                if (x >= 0 && x < Console.BufferWidth && y >= 0 && y < Console.BufferHeight)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(Forma[i]);
                }
            }
            Console.ResetColor();
        }

        public virtual void BorrarDirecto()
        {
            for (int i = 0; i < Forma.Length; i++)
            {
                int x = Posicion.X;
                int y = Posicion.Y + i;

                if (x >= 0 && x < Console.BufferWidth && y >= 0 && y < Console.BufferHeight)
                {
                    Console.SetCursorPosition(x, y);
                    // Escribir espacios del tamaño de la forma en esa línea
                    Console.Write(new string(' ', Forma[i].Length));
                }
            }
        }
    }
}