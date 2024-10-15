using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_CLiente
{
    internal class Errores
    {
        MainWindow main;
        public Errores(MainWindow _main) { 
            main = _main;
        }
        public bool errorfunc(byte[] data, int dim)
        {
            ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerSalida.Text) - 1);
            ushort nSalida = Convert.ToUInt16(main.tb_NumSalidas.Text);
            Array.Reverse(data,4,2);
            ushort nBytes = (ushort)(BitConverter.ToUInt16(data, 4));
            int nBytesS = nSalida / 8;
            int nBytesT = nBytesS + (nSalida % 8 > 0 ? 1 : 0);
            if (data[6]!= 255)
            {
                main.tb_Errores.AppendText("Error dispositivo equivocado\n");
                return false;
            }
            if(dim != 6+ nBytes)
            {
                main.tb_Errores.AppendText("Error los bytes 4 y 5 de la trama no suman"+ dim+ "suman"+nBytes +"\n");
                return false;
            }
            if (dim <= 11)
            {
                if (data[7] >= 0x81)
                {
                    if (data[8] == 1)
                    {
                        main.tb_Errores.AppendText("Funcion ilegal \n");
                    }
                    if (data[8] == 2)
                    {
                        main.tb_Errores.AppendText("Direccion no disponible \n");
                    }
                    if (data[8] == 3)
                    {
                        main.tb_Errores.AppendText("Valor no valido \n");
                    }
                    if (data[8] == 4)
                    {
                        main.tb_Errores.AppendText("Fallo del dispositivo esclavo \n");
                    }
                    if (data[8] == 5)
                    {
                        main.tb_Errores.AppendText("Acknowledge \n");
                    }
                    if (data[8] == 6)
                    {
                        main.tb_Errores.AppendText("Esclavo ocupado \n");
                    }
                    if (data[8] == 7)
                    {
                        main.tb_Errores.AppendText("NEgfative acknowledge \n");
                    }
                    if (data[8] == 8)
                    {
                        main.tb_Errores.AppendText("Error de paridad \n");
                    }
                    return false;
                }
            }
                return (true);
             

        }
    }

}
