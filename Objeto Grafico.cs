using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naves_Invasoras_2
{
    public class ObjetoGrafico
    {
        protected string[] Forma;
        public Posicion Posicion { get; set; }
        public Tamaño Tamaño { get; set; }
        protected ConsoleColor Color;

        public ObjetoGrafico(string[] forma, ConsoleColor color, Tamaño tamaño, Posicion posicion)
        {
            this.Forma = forma;
            this.Color = color;
            this.Tamaño = tamaño;
            this.Posicion = posicion;
        }

        public virtual void Pintar()
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

        public virtual void Borrar()
        {
            for (int i = 0; i < Forma.Length; i++)
            {
                int x = Posicion.X;
                int y = Posicion.Y + i;

                if (x >= 0 && x < Console.BufferWidth && y >= 0 && y < Console.BufferHeight)
                {
                    Console.SetCursorPosition(x, y);
                    //Console.Write(new string(' ',Forma[i].Length));
                    Console.Write(' ');
                }
            }
        }
    }

}
