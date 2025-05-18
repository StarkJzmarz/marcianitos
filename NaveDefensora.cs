using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naves_Invasoras_2
{
    public class NaveDefensora: Nave
    {
        public NaveDefensora(Posicion posicion)
        : base(
            new string[]
            {
                "          █         ",
                "         ███        ",
                "       █     █      ",
                "     █         █    ",
                "   █             █  ",
                " █                 █",
                
            },
            ConsoleColor.Cyan,
            new Tamaño(20, 6), 
            posicion
        )
        {
        }

        
    }
}
