using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Naves_Invasoras_2;

namespace Naves_Invasoras_2
{
    public class Juego
    {
        private List<Jugador> Jugadores;
        private List<NaveInvasora> NavesInvasoras;
        private bool JuegoActivo;

        public Juego()
        {
            Jugadores = new List<Jugador>();
            NavesInvasoras = new List<NaveInvasora>();
        }

        public void IniciarJuego()
        {
            Console.CursorVisible = false;
            JuegoActivo = true;
            Console.Clear();
            Console.Write("Ingrese el nombre del Jugador 1: ");
            string nombre1 = Console.ReadLine();
            Console.Write("Ingrese el nombre del Jugador 2: ");
            string nombre2 = Console.ReadLine();
            Console.Clear();

            NaveDefensora navedef1 = new NaveDefensora(new Posicion(15, Console.BufferHeight - 10));
            NaveDefensora navedef2 = new NaveDefensora(new Posicion(85, Console.BufferHeight - 10));

            Jugadores.Add(new Jugador(nombre1, new Posicion(5, Console.BufferHeight - 1), navedef1));
            Jugadores.Add(new Jugador(nombre2, new Posicion(100, Console.BufferHeight - 1), navedef2));

            int spacingX = 20;
            int spacingY = 6;

            for (int fila = 0; fila < 3; fila++)
            {
                for (int i = 0; i < 10; i++)
                {
                    int escudo = fila + 1;
                    var pos = new Posicion(i * spacingX, fila * spacingY);
                    var nave = new NaveInvasora(pos, fila + 1);
                    NavesInvasoras.Add(nave);
                    nave.Pintar();
                }
            }

            new Thread(MoverNavesContinuamente).Start();
            MostrarInformacionJugadores();

            while (JuegoActivo)
            {
                if (Console.KeyAvailable)
                {
                    var tecla = Console.ReadKey(true).Key;
                    switch (tecla)
                    {
                        case ConsoleKey.A:
                            Jugadores[0].Nave.Mover(Direccion.Izquierda);
                            break;
                        case ConsoleKey.D:
                            Jugadores[0].Nave.Mover(Direccion.Derecha);
                            break;


                        case ConsoleKey.LeftArrow:
                            Jugadores[1].Nave.Mover(Direccion.Izquierda);
                            break;
                        case ConsoleKey.RightArrow:
                            Jugadores[1].Nave.Mover(Direccion.Derecha);
                            break;

                    }
                }

                if (NavesInvasoras.Count == 0)
                {
                    JuegoActivo = false;
                    MostrarPuntajes();
                }
            }
        }
        public void MostrarInformacionJugadores()
        {

            foreach (var jugador in Jugadores)
            {

                Posicion posicion = jugador.posicion;
                Console.SetCursorPosition(posicion.X, posicion.Y);
                Console.Write($"{jugador.GetNombre()} - Puntos: {jugador.GetPuntaje()}");
            }
        }
        public void MostrarPuntajes()
        {
            Console.Clear();
            Console.WriteLine("\n\n----- RESULTADOS FINALES -----\n");
            foreach (var jugador in Jugadores)
            {
                Console.WriteLine($"{jugador.GetNombre()} - Puntos: {jugador.GetPuntaje()}");
            }

            var ganador = Jugadores.OrderByDescending(j => j.GetPuntaje()).First();
            Console.WriteLine($"\n¡Ganador: {ganador.GetNombre()}!\n");

            Console.WriteLine("¿Deseas jugar de nuevo? (S/N)");
            var tecla = Console.ReadKey().Key;
            if (tecla == ConsoleKey.S)
            {
                Console.Clear();
                new Juego().IniciarJuego();
            }
            else
            {
                Console.WriteLine("\nGracias por jugar Invasores del Espacio!");
            }
        }
        private void MoverNavesContinuamente()
        {
            bool haciaDerecha = false;

            while (JuegoActivo)
            {

                if (NavesInvasoras.Any(n => n.Posicion.X <= 0))
                    haciaDerecha = true;
                else if (NavesInvasoras.Any(n => n.Posicion.X + n.Tamaño.Ancho >= Console.WindowWidth))
                    haciaDerecha = false;

                foreach (var nave in NavesInvasoras)
                {
                    nave.Borrar();
                    nave.Mover(haciaDerecha ? Direccion.Derecha : Direccion.Izquierda);
                    nave.Pintar();
                }

                Thread.Sleep(400);
            }
        }
    }

}