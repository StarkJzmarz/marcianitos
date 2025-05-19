// Juego.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; // Para StringBuilder en Renderizar
using System.Threading; // Cambiado de System.Threading.Tasks

namespace Naves_Invasoras_2
{
    public class Juego
    {
        private List<Jugador> Jugadores;
        private List<NaveInvasora> NavesInvasoras;
        private List<Bala> BalasActivas;
        private bool JuegoActivo;

        // Para el doble búfer
        // Se inicializarán en IniciarJuego o Constructor
        private static char[,] screenBuffer;
        private static ConsoleColor[,] colorBuffer;
        private static int bufferAncho;
        private static int bufferAlto;

        // Para controlar la velocidad de movimiento de las naves invasoras
        private long ultimoMovimientoInvasorasTicks;
        private const int INTERVALO_MOVIMIENTO_INVASORAS_MS = 400; // 400ms
        private long ultimoMovimientoBalasTicks;
        private const int INTERVALO_MOVIMIENTO_BALAS_MS = 60;


        private bool direccionFlotaDerecha = true; // Para MoverNavesContinuamente

        public Juego()
        {
            Jugadores = new List<Jugador>();
            NavesInvasoras = new List<NaveInvasora>();
            BalasActivas = new List<Bala>();
            // Es mejor inicializar el buffer con las dimensiones de la consola
            // Lo haremos en IniciarJuego para asegurar que la consola esté lista
        }

        private void InicializarBuffers()
        {
            bufferAncho = Console.WindowWidth;
            bufferAlto = Console.WindowHeight;
            ObjetoGrafico.InicializarBuffer(bufferAncho, bufferAlto); // Llama al método estático
            // Para acceder directamente si es necesario (aunque ObjetoGrafico ya tiene referencia)
            screenBuffer = ObjetoGrafico.screenBuffer;
            colorBuffer = ObjetoGrafico.colorBuffer;
        }


        public void IniciarJuego()
        {
            Console.CursorVisible = false;
            JuegoActivo = true;
            Console.Clear(); // Limpia la consola una vez al inicio

            // Inicializar el buffer después de cualquier Clear y antes de pintar
            InicializarBuffers();


            Console.SetCursorPosition(0, 0); // Para que los ReadLine no queden raros
            Console.Write("Ingrese el nombre del Jugador 1: ");
            string nombre1 = Console.ReadLine();
            Console.Write("Ingrese el nombre del Jugador 2: ");
            string nombre2 = Console.ReadLine();
            //Console.Clear(); // No es necesario si vamos a redibujar todo con el buffer

            // Asegúrate que las posiciones iniciales estén dentro de los límites del buffer/consola
            int alturaSpawnNavesDefensoras = bufferAlto - 5; // Ejemplo, ajusta según el tamaño de tu nave
            int alturaSpawnInfoJugador = bufferAlto - 1;


            //NaveDefensora navedef1 = new NaveDefensora(new Posicion(15, alturaSpawnNavesDefensoras));
            //NaveDefensora navedef2 = new NaveDefensora(new Posicion(bufferAncho - 15 - navedef2.Tamaño.Ancho, alturaSpawnNavesDefensoras)); // Ajusta para que no se salga por la derecha


            NaveDefensora navedef1 = new NaveDefensora(new Posicion(15, alturaSpawnNavesDefensoras));

            // Calcula la posición X para navedef2 usando el tamaño estático
            int anchoNaveDefensora = NaveDefensora.TamañoBase.Ancho;
            int posX_navedef2 = bufferAncho - 15 - anchoNaveDefensora;
            NaveDefensora navedef2 = new NaveDefensora(new Posicion(posX_navedef2, alturaSpawnNavesDefensoras));

            navedef1.setColor(ConsoleColor.Green);
            navedef2.setColor(ConsoleColor.Cyan);


            Jugadores.Add(new Jugador(nombre1, new Posicion(5, alturaSpawnInfoJugador), navedef1));
            Jugadores.Add(new Jugador(nombre2, new Posicion(bufferAncho - 25, alturaSpawnInfoJugador), navedef2)); // Ajustar X para info jugador 2

            int spacingX = 5; // Reducido para que quepan más
            int spacingY = 3; // Reducido
            int navesPorFila = (bufferAncho - 10) / (NaveInvasora.FormaBase[0].Length + spacingX); // Calcular cuántas caben

            for (int fila = 0; fila < 3; fila++)
            {
                for (int i = 0; i < navesPorFila; i++)
                {
                    // La posición de la nave invasora necesita ser relativa al tamaño de la nave
                    // Asumiendo que NaveInvasora tiene un Tamaño definido o podemos obtenerlo de FormaBase
                    int posX = 5 + i * (NaveInvasora.FormaBase[0].Length + spacingX);
                    int posY = 1 + fila * (NaveInvasora.FormaBase.Length + spacingY);
                    var pos = new Posicion(posX, posY);
                    var nave = new NaveInvasora(pos, fila);
                    NavesInvasoras.Add(nave);
                    //nave.PintarEnBuffer(); // Se pintará en el bucle de renderizado
                }
            }

            // Ya no necesitamos un hilo separado para mover y pintar las naves.
            // Lo haremos en el bucle principal.
            // new Thread(MoverNavesContinuamente).Start();
            ultimoMovimientoInvasorasTicks = Environment.TickCount64;


            // Bucle principal del juego
            while (JuegoActivo)
            {
                long TicksAhora = Environment.TickCount64;

                // 1. Procesar Input
                ProcesarInput();

                // 2. Actualizar Estado del Juego
                // Mover naves invasoras a intervalos
                if (TicksAhora - ultimoMovimientoInvasorasTicks > INTERVALO_MOVIMIENTO_INVASORAS_MS)
                {
                    ActualizarMovimientoInvasoras();
                    ultimoMovimientoInvasorasTicks = TicksAhora;
                }

                if (TicksAhora - ultimoMovimientoBalasTicks > INTERVALO_MOVIMIENTO_BALAS_MS)
                {
                    ActualizarMovimientoBalas();
                    ultimoMovimientoBalasTicks = TicksAhora;
                }

                VerificarColisiones();
                LimpiarBalasInactivas();
                // (Aquí iría lógica de colisiones, disparos, etc.)

                // 3. Renderizar
                Renderizar();

                // 4. Comprobar condiciones de fin de juego
                if (NavesInvasoras.Count == 0)
                //if(true)
                {
                    JuegoActivo = false;
                    //Renderizar(); // Renderizar una última vez para mostrar pantalla vacía
                    // así que no interfiere con el buffer si se llama después del bucle.
                }

                // Control de FPS (aprox 30 FPS si el resto toma poco tiempo)
                Thread.Sleep(16); // 1000ms / 30fps ~= 33ms
            }

            MostrarPuntajes(); // MostrarPuntajes ahora usa Console.Write directamente
        }

        private void ProcesarInput()
        {
            if (Console.KeyAvailable)
            {
                var tecla = Console.ReadKey(true).Key;
                Bala nuevaBala = null;
                // Borrar la nave del jugador de su posición ANTIGUA en el buffer
                // Jugadores[0].Nave.BorrarDeBuffer();
                // Jugadores[1].Nave.BorrarDeBuffer(); // Si es necesario

                switch (tecla)
                {
                    case ConsoleKey.A:
                        Jugadores[0].Nave.Mover(Direccion.Izquierda);
                        break;
                    case ConsoleKey.D:
                        Jugadores[0].Nave.Mover(Direccion.Derecha);
                        break;

                    case ConsoleKey.W:
                        if (Jugadores.Count > 0 && Jugadores[0].Nave is NaveDefensora nd1)
                        {
                            nuevaBala = nd1.IntentarDisparar(Jugadores[0]);
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        Jugadores[1].Nave.Mover(Direccion.Izquierda);
                        break;
                    case ConsoleKey.RightArrow:
                        Jugadores[1].Nave.Mover(Direccion.Derecha);
                        break;
                    case ConsoleKey.UpArrow:
                        if (Jugadores.Count > 1 && Jugadores[1].Nave is NaveDefensora nd2)
                        {
                            nuevaBala = nd2.IntentarDisparar(Jugadores[1]);
                        }
                        break;
                    case ConsoleKey.Escape: JuegoActivo = false; break;
                        // case ConsoleKey.Escape: JuegoActivo = false; break; // Salir
                }

                if (nuevaBala != null)
                {
                    BalasActivas.Add(nuevaBala); // Añadir la bala a la lista general
                }
            }
        }
        private void ActualizarMovimientoBalas()
        {
            foreach (Bala bala in BalasActivas)
            {
                if (bala.Activa) // Solo mover si está activa
                {
                    bala.Mover();
                } else
                {
                    bala.setColor(Console.BackgroundColor);
                }
            }
        }

        private void LimpiarBalasInactivas()
        {
            // Eliminar balas que ya no están activas (salieron de pantalla o impactaron)
            // Es importante iterar hacia atrás o usar Linq para evitar problemas al modificar la lista
            BalasActivas.RemoveAll(b => !b.Activa);
        }

        private void VerificarColisiones()
        {
            List<NaveInvasora> invasorasARemover = new List<NaveInvasora>();

            foreach (Jugador jugadorActual in Jugadores) // Renombrado para claridad
            {
                // Acceder a la bala a través de la NaveDefensora del jugador
                Bala balaDelJugador = jugadorActual.Nave.bala;
                // Lista para naves que serán removidas del juego después de este ciclo de colisiones

                if (balaDelJugador != null && balaDelJugador.Activa)
                {
                    Posicion posBala = balaDelJugador.Posicion;

                    foreach (NaveInvasora invasora in NavesInvasoras)
                    {
                        if (!invasora.EstaActiva) continue;

                        if (posBala.X >= invasora.Posicion.X &&
                            posBala.X <= invasora.Posicion.X + invasora.Tamaño.Ancho &&
                            posBala.Y >= invasora.Posicion.Y &&
                            posBala.Y <= invasora.Posicion.Y + invasora.Tamaño.Alto - 3)
                        {
                            balaDelJugador.Activa = false; // Desactivar la bala

                            bool fueDestruida = invasora.RecibirImpacto(); // La nave procesa el impacto



                            if (fueDestruida)
                            {
                                // Si fue destruida, añadir a la lista para removerla del juego
                                invasorasARemover.Add(invasora);
                                // Dar puntos por destrucción
                                if (balaDelJugador.Propietario != null)
                                {
                                    // Puntaje diferente si se destruye una nave más fuerte
                                    int puntosPorNave = 1; // Base
                                    balaDelJugador.Propietario.IncrementarPuntaje(puntosPorNave);
                                }
                            }
                            else
                            {
                                // La nave fue dañada pero no destruida.
                                // Podrías dar menos puntos por solo dañar, o ninguno.
                                if (balaDelJugador.Propietario != null)
                                {
                                    // balaDelJugador.Propietario.IncrementarPuntaje(5); // Puntos por daño
                                }
                                // La nave cambia de color en su método RecibirImpacto si así lo programaste.
                            }

                            break; // La bala solo impacta una nave por frame/movimiento
                        }
                    }
                }
            }

            // Remover todas las naves marcadas como destruidas de la lista principal
            foreach (var invasora in invasorasARemover)
            {
                NavesInvasoras.Remove(invasora);
            }
        }

        private void ActualizarMovimientoInvasoras()
        {
            if (!NavesInvasoras.Any()) return;

            bool llegoBorde = false;

            // Primero, determinar si alguna nave ha llegado al borde
            foreach (var nave in NavesInvasoras)
            {
                if (direccionFlotaDerecha)
                {
                    if (nave.Posicion.X + nave.Tamaño.Ancho >= bufferAncho - 15)
                    {
                        llegoBorde = true;
                        break;
                    }
                }
                else // Moviendo hacia la izquierda
                {
                    if (nave.Posicion.X <= 5)
                    {
                        llegoBorde = true;
                        break;
                    }
                }
            }

            if (llegoBorde)
            {
                direccionFlotaDerecha = !direccionFlotaDerecha;
            }

            // Mover todas las naves en la dirección actual
            // Es importante hacer esto en un bucle separado DESPUÉS de decidir si cambiar dirección y descender
            foreach (var nave in NavesInvasoras)
            {
                // nave.BorrarDeBuffer(); // Opcional si no limpias el buffer completo
                nave.Mover(direccionFlotaDerecha ? Direccion.Derecha : Direccion.Izquierda);
            }
        }

        private void Renderizar()
        {
            ObjetoGrafico.LimpiarBuffer(); // Asegúrate que LimpiarBuffer ponga el color de fondo deseado
                                           // en colorBuffer (ej. Console.BackgroundColor o ConsoleColor.Black)

            foreach (var jugador in Jugadores)
            {
                jugador.Nave.PintarEnBuffer();
            }

            List<NaveInvasora> navesParaPintar = new List<NaveInvasora>(NavesInvasoras);
            foreach (var nave in navesParaPintar)
            {
                nave.PintarEnBuffer();
            }

            // Pintar todas las balas activas
            foreach (Bala bala in BalasActivas)
            {
                if (bala.Activa) // Aunque Mover ya podría haberla desactivado, una doble comprobación no hace daño
                {
                    bala.PintarEnBuffer();
                }
            }

            PintarInfoJugadoresEnBuffer();

            StringBuilder sb = new StringBuilder(bufferAncho * bufferAlto + bufferAlto);
            Console.SetCursorPosition(0, 0);

            // Inicializar 'colorAnteriorConsola' con un valor que GARANTICE que sea diferente
            // al primer color real que se encuentre en el buffer.
            // El truco de usar un valor de enum inválido es común para esto.
            ConsoleColor colorAnteriorConsola = (ConsoleColor)(-1); // Un valor que no es un ConsoleColor válido

            for (int y = 0; y < bufferAlto; y++)
            {
                for (int x = 0; x < bufferAncho; x++)
                {
                    ConsoleColor colorCeldaActualBuffer = colorBuffer[y, x];

                    if (colorCeldaActualBuffer != colorAnteriorConsola)
                    {
                        // Si estamos usando StringBuilder, esta parte es para construir la cadena
                        // con los códigos de escape de color ANSI si la consola los soporta
                        // (más complejo y no estándar en Console de .NET directamente)
                        // o simplemente cambiar el color de la consola antes de añadir al SB.
                        // Para la consola estándar de .NET, cambiamos el estado de la consola.

                        // Esta es la lógica que se aplica a la CONSOLA REAL, no al StringBuilder directamente.
                        // Cuando Console.Write(sb.ToString()) ocurra, la consola usará el último
                        // ForegroundColor establecido. PERO ESTO ES INCORRECTO para StringBuilder.

                        // ¡ERROR EN MI LÓGICA ANTERIOR CON STRINGBUILDER Y CAMBIO DE COLOR!
                        // Si usamos StringBuilder para TODA la pantalla, los cambios de Console.ForegroundColor
                        // deben ocurrir ANTES de Console.Write(sb.ToString()) y de forma que afecten a la
                        // escritura real. La forma más simple es NO usar StringBuilder si cambiamos color
                        // por celda, o usar StringBuilder por segmentos de color.

                        // VAMOS A VOLVER A LA ESCRITURA DIRECTA CELDA POR CELDA CON LA OPTIMIZACIÓN,
                        // YA QUE ES MÁS DIRECTO PARA LA CONSOLA DE .NET SIN ANSI ESCAPES.

                        // La versión con StringBuilder y optimización de color es más compleja porque
                        // Console.ForegroundColor afecta el estado global, no lo que se mete al SB.

                        // CORRECCIÓN: Aplicar el cambio de color y escribir directamente.
                        // O construir el StringBuilder de forma diferente (más abajo).
                    }
                    // sb.Append(screenBuffer[y, x]); // Se mueve esta línea
                }
                // if (y < bufferAlto - 1)
                // {
                //    sb.AppendLine();
                // }
            }
            // Console.Write(sb.ToString()); // Esta línea se mueve o cambia


            // --- MÉTODO REVISADO Y CORRECTO CON OPTIMIZACIÓN DE COLOR Y ESCRITURA DIRECTA ---
            // (Sacrificamos un poco la idea de "un solo Console.Write" por la correcta aplicación de color)

            Console.SetCursorPosition(0, 0); // Asegurar que empezamos arriba
            colorAnteriorConsola = (ConsoleColor)(-1); // Reiniciar para este enfoque

            for (int y = 0; y < bufferAlto; y++)
            {
                // Para este enfoque, no necesitamos StringBuilder para toda la pantalla si cambiamos colores frecuentemente.
                // O, si usamos StringBuilder, lo hacemos por segmentos de color.
                // Por simplicidad y para asegurar que la optimización de color funcione:
                Console.SetCursorPosition(0, y); // Mover al inicio de la línea actual

                for (int x = 0; x < bufferAncho; x++)
                {
                    ConsoleColor colorCeldaActualBuffer = colorBuffer[y, x];

                    if (colorCeldaActualBuffer != colorAnteriorConsola)
                    {
                        Console.ForegroundColor = colorCeldaActualBuffer;
                        colorAnteriorConsola = colorCeldaActualBuffer;
                    }
                    Console.Write(screenBuffer[y, x]); // Escribir el carácter directamente
                }
                // No necesitamos Console.WriteLine() aquí porque ya estamos manejando
                // SetCursorPosition para cada línea. Si la terminal no hace bien el wrap,
                // podrías necesitarlo, pero SetCursorPosition(0,y) suele ser suficiente.
            }
            // --- FIN DEL MÉTODO REVISADO ---
        }


        public void PintarInfoJugadoresEnBuffer()
        {
            foreach (var jugador in Jugadores)
            {
                string info = $"{jugador.GetNombre()} - Puntos: {jugador.GetPuntaje()}";
                Posicion posInfo = jugador.posicion; // La posición base para la info del jugador

                for (int i = 0; i < info.Length; i++)
                {
                    int bufferX = posInfo.X + i;
                    int bufferY = posInfo.Y;

                    if (bufferY >= 0 && bufferY < bufferAlto &&
                        bufferX >= 0 && bufferX < bufferAncho)
                    {
                        screenBuffer[bufferY, bufferX] = info[i];
                        colorBuffer[bufferY, bufferX] = ConsoleColor.White; // O el color que prefieras
                    }
                }
            }
        }


        public void MostrarPuntajes()
        {
            Console.Clear(); // Limpia la pantalla específicamente para los puntajes
            Console.SetCursorPosition(0, 2); // Un poco de margen superior
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("----- RESULTADOS FINALES -----");
            Console.WriteLine(); // Línea en blanco

            Console.ForegroundColor = ConsoleColor.White;
            if (Jugadores.Any())
            {
                foreach (var jugador in Jugadores.OrderByDescending(j => j.GetPuntaje())) // Ordenar por puntaje
                {
                    Console.WriteLine($"{jugador.GetNombre(),-20} - Puntos: {jugador.GetPuntaje()}");
                }

                var ganador = Jugadores.OrderByDescending(j => j.GetPuntaje()).First();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"¡Ganador: {ganador.GetNombre()} con {ganador.GetPuntaje()} puntos!");
            }
            else
            {
                Console.WriteLine("No hubo jugadores en esta partida.");
            }
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("¿Deseas jugar de nuevo? (S/N)");

            ConsoleKeyInfo teclaInfo;
            do // Esperar hasta que se presione S o N
            {
                teclaInfo = Console.ReadKey(true);
            } while (teclaInfo.Key != ConsoleKey.S && teclaInfo.Key != ConsoleKey.N);


            if (teclaInfo.Key == ConsoleKey.S)
            {
                // Reiniciar estado del juego para una nueva partida
                // Es mejor tener un método Reset() o reiniciar las listas aquí
                // antes de llamar a IniciarJuego() recursivamente o en un nuevo objeto.
                Jugadores.Clear();
                NavesInvasoras.Clear();
                // BalasActivas.Clear(); // Si tenías una lista global de balas
                // Otras variables de estado que necesiten reinicio.

                // Opción 1: Crear una nueva instancia de Juego (más simple si no hay estado global importante)
                // new Juego().IniciarJuego();

                // Opción 2: Llamar a IniciarJuego en la misma instancia (preferible si quieres mantener estadísticas globales o similar)
                //          Esto requiere que IniciarJuego esté preparado para reinicializar todo.
                //          Tu IniciarJuego actual parece bastante autocontenido para esto.
                IniciarJuego(); // Llama recursivamente. Funciona pero puede consumir stack en muchas partidas.
                                // Una mejor estructura sería un bucle en Main() que cree `new Juego().IniciarJuego()`.
            }
            else
            {
                Console.Clear();
                Console.SetCursorPosition(0, 5);
                Console.WriteLine("Gracias por jugar Invasores del Espacio!");
                Thread.Sleep(2000);
            }
        }
        // Este método ya no se usa como hilo, se integró en el bucle principal
        // private void MoverNavesContinuamente() { ... }
    }
}