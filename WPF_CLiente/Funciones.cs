using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPF_CLiente
{
    internal class Funciones
    {
        private MainWindow main;
       public Funciones(MainWindow _main) {
        main = _main;
        
        }
        public byte[] primerafuncion() //Leer salida 1
        {
            byte[] peticion = new byte[12];
            ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerSalida.Text) - 1);
            ushort nSalida = Convert.ToUInt16(main.tb_NumSalidas.Text);
            int nBytesS = nSalida / 8;
            int nBytesT = nBytesS + (nSalida % 8 > 0 ? 1 : 0);
            byte[] temp;
            temp = BitConverter.GetBytes(nBytesT);
            Array.Copy(temp, 0, peticion, 0, 2); 
            peticion[2] = peticion[3] = 0; //Estos son 0
            temp = BitConverter.GetBytes((ushort)6);
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 4, 2);
            peticion[6] = 22;
            //Funcion
            peticion[7] = 1;
            temp = BitConverter.GetBytes(primerasalida);
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 8, 2);
            temp = BitConverter.GetBytes(nSalida);
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 10, 2);
            return peticion;
            //BIEN
        }
            public byte[] quintafuncion() //Escribir salida 5
            {
                byte[] peticion = new byte[12];
                ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerSalida.Text) - 1);
                ushort nSalida = Convert.ToUInt16(main.tb_valoresS.Text);
            if(nSalida> 0) nSalida = 0xFF00;
            else nSalida = 0;
                int nBytesS = nSalida / 8;
                int nBytesT = nBytesS + (nSalida % 8 > 0 ? 1 : 0);
                byte[] temp;
                temp = BitConverter.GetBytes(nBytesT);
                Array.Copy(temp, 0, peticion, 0, 2);
                peticion[2] = peticion[3] = 0;
                temp = BitConverter.GetBytes((ushort)6);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 4, 2);
                peticion[6] = 22;
            //Funcion
                peticion[7] = 5;
                temp = BitConverter.GetBytes(primerasalida);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 8, 2);
                temp = BitConverter.GetBytes(nSalida);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 10, 2);
                return peticion;
            //Bien
            }
            public byte[] segundafuncion() //Leer entrada 2
            {
                byte[] peticion = new byte[12];
                ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerEntradas.Text) - 1);
                ushort nSalida = Convert.ToUInt16(main.tb_NumEntradas.Text);
                int nBytesS = nSalida / 8;
                int nBytesT = nBytesS + (nSalida % 8 > 0 ? 1 : 0);
                byte[] temp;
                temp = BitConverter.GetBytes(nBytesT);
                Array.Copy(temp, 0, peticion, 0, 2);
                peticion[2] = peticion[3] = 0;
                temp = BitConverter.GetBytes((ushort)6);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 4, 2);
                peticion[6] = 22;
                 //Funcion
                peticion[7] = 2;
                temp = BitConverter.GetBytes(primerasalida);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 8, 2);
                temp = BitConverter.GetBytes(nSalida);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 10, 2);
                return peticion;
                //Bien
            }
            public byte[] decimoquintafuncion() //Escribir Entrada 15
            {
                
                ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerSalida.Text) - 1);
                ushort nSalidas = Convert.ToUInt16(main.tb_NumSalidas.Text);
                int nBytesS = nSalidas / 8;
                int nBytesT = nBytesS + (nSalidas % 8 > 0 ? 1 : 0);
                byte[] peticion = new byte[13+nBytesT];
                byte[] temp;
                temp = BitConverter.GetBytes(nBytesT);
                Array.Copy(temp, 0, peticion, 0, 2);
                peticion[2] = peticion[3] = 0;
                temp = BitConverter.GetBytes((ushort)7 + nBytesT);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 4, 2);
                peticion[6] = 22;
                //Funcion
                peticion[7] = 15;
                temp = BitConverter.GetBytes(primerasalida);
               Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 8, 2);
                temp = BitConverter.GetBytes(nSalidas);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 10, 2);           
                temp = BitConverter.GetBytes(nBytesT);
                Array.Copy(temp, 0, peticion, 12, 1);
            for (int indexer = 0; indexer < nBytesT; indexer++)
            {
                //Pasar de 10110 a valores

                peticion[13 + indexer] = BitstoByte(indexer);
            }
            return peticion;
            //Hecho
            }
        //Arreglar mañana
        private byte BitstoByte(int n)
        {
         string valores = main.tb_valoresS.Text.ToString();
            byte sal = 0;
            uint valor = 0;
            bool[] unos = new bool[8];
            for (int i = n*8; i < 8*(n+1); i++)
            {
                if(i < valores.Length) { 
                 valor = Convert.ToUInt16(valores.Substring(i,1));
                }
                else
                {
                    valor = 0;
                }
                if (valor == 1)
                {
                    unos[i-n*8] = true;
                }
                else
                {
                    unos[i-n*8] = false;
                }

            }
            for (int j = 0; j < 8; ++j)
            {
                sal |= (byte)((unos[j] ? 1 : 0) << j);
            }
            return sal;
        }
        public byte[] tercerafuncion() //Leer registro 3
            {
                byte[] peticion = new byte[12];
                ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerRegistro.Text) - 1);
                ushort nSalida = Convert.ToUInt16(main.tb_NumRegistros.Text);
                int nBytesS = nSalida / 8;
                int nBytesT = nBytesS + (nSalida % 8 > 0 ? 1 : 0);
                byte[] temp;
                temp = BitConverter.GetBytes(nBytesT);
                Array.Copy(temp, 0, peticion, 0, 2);
                peticion[2] = peticion[3] = 0;
                temp = BitConverter.GetBytes((ushort)6);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 4, 2);
                peticion[6] = 22;
                //Funcion
                peticion[7] = 3;
                temp = BitConverter.GetBytes(primerasalida);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 8, 2);
                temp = BitConverter.GetBytes(nSalida);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 10, 2);
                return peticion;
            //Bien
            }
        public byte[] cuartafuncion() //Leer registro 4
        {
            byte[] peticion = new byte[12];
            ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerRegistroE.Text) - 1);
            ushort nSalida = Convert.ToUInt16(main.tb_NumRegistrosE.Text);
            int nBytesS = nSalida / 8;
            int nBytesT = nBytesS + (nSalida % 8 > 0 ? 1 : 0);
            byte[] temp;
            temp = BitConverter.GetBytes(nBytesT);
            Array.Copy(temp, 0, peticion, 0, 2);
            peticion[2] = peticion[3] = 0;
            temp = BitConverter.GetBytes((ushort)6);
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 4, 2);
            peticion[6] = 22;
            //Funcion
            peticion[7] = 4;
            temp = BitConverter.GetBytes(primerasalida);
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 8, 2);
            temp = BitConverter.GetBytes(nSalida);
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 10, 2);
            return peticion;
            //Bien
        }
        public byte[] sextafuncion() //Escribir Registro 6
            {
                byte[] peticion = new byte[12];
                ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerRegistro.Text) - 1);
                ushort nSalida = Convert.ToUInt16(main.sld_uno.Value);
                int nBytesS = nSalida / 8;
                int nBytesT = nBytesS + (nSalida % 8 > 0 ? 1 : 0);
                byte[] temp;
                temp = BitConverter.GetBytes(nBytesT);
                Array.Copy(temp, 0, peticion, 0, 2);
                peticion[2] = peticion[3] = 0;
                temp = BitConverter.GetBytes((ushort)6);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 4, 2);
                peticion[6] = 22;
                //Funcion
                peticion[7] = 6;
                temp = BitConverter.GetBytes(primerasalida);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 8, 2);
                temp = BitConverter.GetBytes(nSalida);
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 10, 2);
                return peticion;
            //Bien
            }
        public byte[] septimafuncion() //Leer Error 7 
        {
            byte[] peticion = new byte[8];
            ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerRegistro.Text) - 1);
            ushort nSalida = Convert.ToUInt16(main.sld_uno.Value);
            int nBytesS = nSalida / 8;
            int nBytesT = nBytesS + (nSalida % 8 > 0 ? 1 : 0);
            byte[] temp;
            temp = BitConverter.GetBytes(nBytesT);
            Array.Copy(temp, 0, peticion, 0, 2);
            peticion[2] = peticion[3] = 0;
            temp = BitConverter.GetBytes((ushort)2);
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 4, 2);
            peticion[6] = 22;
            //Funcion
            peticion[7] = 7;
            return peticion;
            //Bien
        }
        //FUNCION 16
        public byte[] decimosextafuncion() //Escribir Registro 16
        {
            ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerRegistro.Text) - 1);
            ushort nSalida = Convert.ToUInt16(main.sld_uno.Value);
            ushort nSalidas = Convert.ToUInt16(main.tb_NumRegistros.Text);

            int nBytesS = nSalida / 8;
            int nBytesT = nBytesS + (nSalida % 8 > 0 ? 1 : 0);
            byte[] peticion = new byte[13+2* nSalidas];

            byte[] temp;
            temp = BitConverter.GetBytes(nSalidas);
            Array.Copy(temp, 0, peticion, 0, 2);
            peticion[2] = peticion[3] = 0;
            temp = BitConverter.GetBytes((ushort)(7+2*(nSalidas)));
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 4, 2);
            peticion[6] = 22;
            //Funcion
            peticion[7] = 16;
            temp = BitConverter.GetBytes(primerasalida);
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 8, 2);
            temp = BitConverter.GetBytes(nSalidas);
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 10, 2);
            temp = BitConverter.GetBytes(nSalidas*2);
            Array.Copy(temp, 0, peticion, 12, 1);
            for(int i = 0; i< nSalidas; i++)
            {
                TextBox box = new TextBox();
                if (i == 0)
                {
                    box = main.tb_1;
                }
                if (i == 1)
                {
                    box = main.tb_2;
                }
                if (i == 2)
                {
                    box = main.tb_3;
                }
                if (i == 3)
                {
                    box = main.tb_4;
                }
                if (i == 4)
                {
                    box = main.tb_5;
                }
                if (i == 5)
                {
                    box = main.tb_6;
                }
                if (i == 6)
                {
                    box = main.tb_7;
                }
                if (i == 7)
                {
                    box = main.tb_8;
                }
                temp = BitConverter.GetBytes(Convert.ToUInt16(box.Text));
                Array.Reverse(temp, 0, 2);
                Array.Copy(temp, 0, peticion, 13+i*2, 2);
            }
            return peticion;
            //Hecho
        }
        public byte[] undecimafuncion() //Escribir Registro 11
        {
            byte[] peticion = new byte[8];
            ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerRegistro.Text) - 1);
            ushort nSalida = Convert.ToUInt16(main.sld_uno.Value);
            int nBytesS = nSalida / 8;
            int nBytesT = nBytesS + (nSalida % 8 > 0 ? 1 : 0);
            byte[] temp;
            temp = BitConverter.GetBytes(nBytesT);
            Array.Copy(temp, 0, peticion, 0, 2);
            peticion[2] = peticion[3] = 0;
            temp = BitConverter.GetBytes((ushort)2);
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 4, 2);
            peticion[6] = 22;
            //Funcion
            peticion[7] = 11;
            return peticion;
            //Hecho
        }
        public byte[] duodecimafuncion() //Escribir Registro 12
        {
            byte[] peticion = new byte[8];
            ushort primerasalida = (ushort)(Convert.ToUInt16(main.tb_PrimerRegistro.Text) - 1);
            ushort nSalida = Convert.ToUInt16(main.sld_uno.Value);
            int nBytesS = nSalida / 8;
            int nBytesT = nBytesS + (nSalida % 8 > 0 ? 1 : 0);
            byte[] temp;
            temp = BitConverter.GetBytes(nBytesT);
            Array.Copy(temp, 0, peticion, 0, 2);
            peticion[2] = peticion[3] = 0;
            temp = BitConverter.GetBytes((ushort)2);
            Array.Reverse(temp, 0, 2);
            Array.Copy(temp, 0, peticion, 4, 2);
            peticion[6] = 22;
            //Funcion
            peticion[7] = 12;

            return peticion;
            //Hecho
        }
    }
}
