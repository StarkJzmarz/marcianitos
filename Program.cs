using System.Runtime.InteropServices; // Necesario para DllImport

namespace Naves_Invasoras_2
{
    internal class Program
    {

        // Importamos la función GetConsoleWindow de kernel32.dll
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        // Importamos la función ShowWindow de user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Constantes para ShowWindow
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_MAXIMIZE = 3; // Maximiza la ventana
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_SHOWNA = 8;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;

        static void Main(string[] args)
        {
            // Obtenemos el handle (identificador) de la ventana de la consola actual
            IntPtr consoleWindow = GetConsoleWindow();

            // Verificamos que obtuvimos un handle válido
            if (consoleWindow != IntPtr.Zero)
            {
                // Maximizamos la ventana de la consola
                ShowWindow(consoleWindow, SW_MAXIMIZE);
            }
            else
            {
                Console.WriteLine("No se pudo obtener el handle de la ventana de la consola.");
                // Esto podría suceder si la aplicación no está adjunta a una consola
                // (por ejemplo, si es una aplicación de Windows Forms/WPF sin consola).
            }

            Console.WriteLine("¡Hola! La consola debería estar maximizada.");
            Console.WriteLine("Presiona cualquier tecla para restaurar y salir...");
            Console.ReadKey();

            // Opcional: Restaurar la ventana a su tamaño normal antes de salir
            if (consoleWindow != IntPtr.Zero)
            {
                Thread.Sleep(100);
                ShowWindow(consoleWindow, SW_SHOWNORMAL); // O SW_RESTORE si prefieres
            }


            Juego juego = new Juego();
            juego.IniciarJuego();
        }
    }
}
