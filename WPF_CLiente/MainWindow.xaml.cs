using WPF_CLiente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;
using System.Reflection;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace WPF_CLiente
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public delegate void delegadoTexto(String texto);

    public partial class MainWindow : Window
    {
        // Campos:

        public delegadoTexto delegadoBanner;
        private Cliente? clie;
        private ClienteTLS? clieTLS;
        private Funciones Func;
        private Errores Error;
        private bool online = false;

        int i = 0;
        public MainWindow()
        {
            InitializeComponent();
            Error = new Errores(this);
            Func = new Funciones(this);
            delegadoBanner = new delegadoTexto(cambiaBanner);
            
        }

        private void Boton_salir(object sender, RoutedEventArgs e)
        {
            if (clie != null) clie.cierraCliente();
            clie = null;
            Close();

        }
        private void Boton_connectar(object sender, RoutedEventArgs e)
        {
            if (!online)
            {
                delegadoBanner = new delegadoTexto(cambiaBanner);
                if ((bool)btn_Seguras.IsChecked) {
                    clieTLS = new ClienteTLS(tb_DirrecionIP.Text, delegadoBanner, tb_CertificadoCliente.Text, tb_CertificadoServidor.Text);
                    clieTLS.hiloConectado.Start();
                }
                else {
                    clie = new Cliente(tb_DirrecionIP.Text, delegadoBanner);
                    clie.hiloConectado.Start();
                }
               
            }
            else
            {
                if (clie != null) clie.cierraCliente();
                clie = null;
                if (clieTLS != null) clieTLS.cierraCliente();
                clieTLS = null;
                btn_Conectar.Content = "Conectar";
                // Añadir Banner
                online = false;
                btn_Entradas.IsEnabled = false;
                btn_Salidas.IsEnabled = false;
                btn_Registros.IsEnabled = false;
                btn_RegistrosE.IsEnabled = false;
                btn_PedirError.IsEnabled = false;
                this.Title = "Cliente TCP Desconectado";
            }
            return;
        }
        //Cambiar funcion
        
        private void cambiaBanner(string Texto)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(delegadoBanner, Texto);
            else
            {
                if (Texto.Length > 0)
                    this.Title = "Cliente TCP(" + Texto + ")";
                else
                    this.Title = "Cliente TCP";
                if (clie != null && clie.online)
                { //Cambiar la condicion
                    btn_Conectar.Content = "Desconectar";
                    btn_Entradas.IsEnabled = true;
                    btn_Salidas.IsEnabled = true;
                    btn_Registros.IsEnabled = true;
                    btn_RegistrosE.IsEnabled = true;

                    btn_PedirError.IsEnabled = true;
                    //Añadir el Banner

                    online = true;

                }
                if (clieTLS != null && clieTLS.online)
                {

                    btn_Conectar.Content = "Desconectar";
                    btn_Entradas.IsEnabled = true;
                    btn_Salidas.IsEnabled = true;
                    btn_Registros.IsEnabled = true;
                    btn_RegistrosE.IsEnabled = true;
                    btn_PedirError.IsEnabled = true;
                    //Añadir el Banner

                    online = true;

                }
            }
        }

        private void btn_Salidas_Click(object sender, RoutedEventArgs e)
        {
            if(clie != null || clieTLS != null)
            {
                byte[] peticion;
                if (!(bool)btn_SEscribir.IsChecked) { 
                peticion = Func.primerafuncion();
                }
                else
                {
                    if ((bool)btn_variasS.IsChecked){
                        peticion = Func.decimoquintafuncion();
                    }
                    else
                    {
                        peticion = Func.quintafuncion();
                    }         
                }
                byte[] respuesta = new byte[256];
                bool err;
                int res = 0;
                if (clie != null)
                   res = clie.enviaDatos(peticion, peticion.Length);
                if (clieTLS != null)
                   res = clieTLS.enviaDatos(peticion, peticion.Length);
                if (res == peticion.Length)
                {
                    if(clie != null)
                    res = clie.recibeDatos(respuesta, respuesta.Length);
                    if(clieTLS != null)
                        res = clieTLS.recibeDatos(respuesta, respuesta.Length);
                    err = Error.errorfunc(respuesta, res);
                    if (err == true)
                    {
                        List<dataGrid> Lista = new List<dataGrid>();
                        dataGrid elemento;
                        bool[] temp;
                        int k = Convert.ToInt32(tb_PrimerSalida.Text);
                        int maxBits;
                        if (!(bool)btn_SEscribir.IsChecked) { 
                            for (int i = 0; i < respuesta[8]; i++)
                            {
                                temp = BytetoBits(respuesta[9 + i], 8);
                                if (i < Convert.ToUInt16(tb_NumSalidas.Text) / 8) maxBits = 8;
                                else maxBits = Convert.ToUInt16(tb_NumSalidas.Text) % 8;
                                for (int j=0; j< maxBits; j++)
                                {
                                    elemento = new dataGrid();
                                    elemento.nSalida = k++;
                                    elemento.estado = temp[j];
                                    Lista.Add(elemento);

                                }
                            }
                        }
                        else
                        {
                            if ((bool)btn_variasS.IsChecked)
                            {
                                peticion = Func.primerafuncion();
                                peticion[8] = respuesta[8];
                                peticion[9] = respuesta[9];
                                peticion[10]= respuesta[10];
                                peticion[11]= respuesta[11];
                                if (clie != null)
                                    res = clie.enviaDatos(peticion, peticion.Length);
                                if (clieTLS != null)
                                    res = clieTLS.enviaDatos(peticion, peticion.Length);
                                if (res == peticion.Length)
                                {
                                    if (clie != null)
                                        res = clie.recibeDatos(respuesta, respuesta.Length);
                                    if (clieTLS != null)
                                        res = clieTLS.recibeDatos(respuesta, respuesta.Length);
                                    err = Error.errorfunc(respuesta, res);
                                    if (err == true)
                                    {
                                        for (int i = 0; i < respuesta[8]; i++)
                                        {
                                            temp = BytetoBits(respuesta[9 + i], 8);
                                            if (i < respuesta[11] / 8) maxBits = 8;
                                            else maxBits = respuesta[11] % 8;
                                            for (int j = 0; j < maxBits; j++)
                                            {
                                                elemento = new dataGrid();
                                                elemento.nSalida = k++;
                                                elemento.estado = temp[j];
                                                Lista.Add(elemento);

                                            }
                                        }
                                    }
                                }
                                //Seguir aqui
                            }
                            else
                            {
                                byte[] temp1 = new byte[2];
                                bool[] temp2 = BytetoBits(respuesta[10], 1);
                                Array.Copy(respuesta, 8, temp1, 0, 2);
                                Array.Reverse(temp1);
                                elemento = new dataGrid();
                                elemento.nSalida = BitConverter.ToInt16(temp1) + 1;
                                elemento.estado = temp2[0];
                                Lista.Add(elemento);
                            }

                        }
                        Data_Salida.ItemsSource = Lista;
                    }
                }


            }
        }
        private bool[] BytetoBits(byte valor, int n)
        {
            bool[] bits = new bool[8];
            byte mascara = 1;
            for(int i= 0; i<n; i++)
            {
                if((valor & mascara) != 0) bits[i] = true;
            else bits[i] = false;
                mascara = (byte)(mascara << 1);
            }
            for (int i = n; i < 8; i++)
            {
                bits[i] = false;
            }
            return bits;
        }
        private void btn_Entradas_Click(object sender, RoutedEventArgs e)
        {
            if (clie != null || clieTLS != null)
            {
                byte[] peticion;

                    peticion = Func.segundafuncion();
              
                byte[] respuesta = new byte[256];
                bool err;
                int res = 0;
                if (clie != null)
                    res = clie.enviaDatos(peticion, 12);
                if (clieTLS != null)
                    res = clieTLS.enviaDatos(peticion, 12);
                if (res == 12)
                {
                    if (clie != null)
                        res = clie.recibeDatos(respuesta, respuesta.Length);
                    if (clieTLS != null)
                        res = clieTLS.recibeDatos(respuesta, respuesta.Length);
                    err = Error.errorfunc(respuesta, res);
                    if (err == true)
                    {
                        List<dataGrid> Lista = new List<dataGrid>();
                        dataGrid elemento;
                        bool[] temp;
                        int k = Convert.ToInt32(tb_PrimerEntradas.Text);
                        int maxBits;

                            for (int i = 0; i < respuesta[8]; i++)
                            {
                                temp = BytetoBits(respuesta[9 + i], 8);
                                if (i < Convert.ToUInt16(tb_NumEntradas.Text) / 8) maxBits = 8;
                                else maxBits = Convert.ToUInt16(tb_NumEntradas.Text) % 8;
                                for (int j = 0; j < maxBits; j++)
                                {
                                    elemento = new dataGrid();
                                    elemento.nSalida = k++;
                                    elemento.estado = temp[j];
                                    Lista.Add(elemento);

                                }
                            }

                   
                        Data_Entradas.ItemsSource = Lista;
                    }
                }
            }
            //Funciona
        }
        private void btn_Registros_Click(object sender, RoutedEventArgs e)
        {
            //Añadir la funcion 16
            if (clie != null || clieTLS != null)
            {
                byte[] peticion;
                if (!(bool)btn_REscribir.IsChecked)
                {
                    peticion = Func.tercerafuncion();
                }
                else
                {
                    if ((bool)btn_varios.IsChecked)
                    {
                        peticion = Func.decimosextafuncion();
                    }
                    else { 
                    peticion = Func.sextafuncion();
                    }
                }
                byte[] respuesta = new byte[256];
                bool err;
                int res = 0;
                if (clie != null)
                    res = clie.enviaDatos(peticion, peticion.Length);
                if (clieTLS != null)
                    res = clieTLS.enviaDatos(peticion, peticion.Length);
                if (res == peticion.Length)
                {
                    if (clie != null)
                        res = clie.recibeDatos(respuesta, respuesta.Length);
                    if (clieTLS != null)
                        res = clieTLS.recibeDatos(respuesta, respuesta.Length);
                    err = Error.errorfunc(respuesta, res);
                    if (err == true)
                    {
                        List<dataGrid> Lista = new List<dataGrid>();
                        dataGrid elemento;
                        byte[] temp = new byte[2];
                        int k = Convert.ToInt32(tb_PrimerRegistro.Text);
                        int maxBits;
                        if (!(bool)btn_REscribir.IsChecked)
                        {

                                for (int j = 0; j < Convert.ToUInt16(tb_NumRegistros.Text); j++)
                                {
                                    elemento = new dataGrid();
                                    elemento.nSalida = k++;
                                    Array.Copy(respuesta,8+((k)-1)*2, temp, 0, 2);
                                    //Array.Reverse(temp);
                                    elemento.estadoR = BitConverter.ToInt16(temp);
                                    Lista.Add(elemento);

                                }
                            
                        }
                        else
                        {
                            if ((bool)btn_varios.IsChecked)
                            {
                                peticion = Func.tercerafuncion();
                                peticion[8] = respuesta[8];
                                peticion[9] = respuesta[9];
                                peticion[10] = respuesta[10];
                                peticion[11] = respuesta[11];
                                if (clie != null)
                                    res = clie.enviaDatos(peticion, peticion.Length);
                                if (clieTLS != null)
                                    res = clieTLS.enviaDatos(peticion, peticion.Length);
                                if (res == peticion.Length)
                                {
                                    if (clie != null)
                                        res = clie.recibeDatos(respuesta, respuesta.Length);
                                    if (clieTLS != null)
                                        res = clieTLS.recibeDatos(respuesta, respuesta.Length);
                                    err = Error.errorfunc(respuesta, res);
                                    if (err == true)
                                    {

                                            for (int j = 0; j < Convert.ToUInt16(tb_NumRegistros.Text); j++)
                                            {
                                                elemento = new dataGrid();
                                                elemento.nSalida = k++;
                                                Array.Copy(respuesta, 8 + ((k) - 1) * 2, temp, 0, 2);
                                                //Array.Reverse(temp);
                                                elemento.estadoR = BitConverter.ToInt16(temp);
                                                Lista.Add(elemento);

                                            }
                                        
                                    }
                                }
                            }
                            else
                            {
                                byte[] temp1 = new byte[2];
                                Array.Copy(respuesta, 8, temp1, 0, 2);
                                Array.Reverse(temp1);
                                elemento = new dataGrid();
                                elemento.nSalida = BitConverter.ToInt16(temp1) + 1;
                                Array.Copy(respuesta, 10, temp1, 0, 2);
                                Array.Reverse(temp1);
                                elemento.estadoR = BitConverter.ToInt16(temp1);
                                Lista.Add(elemento);
                            }
                            
                        }
                        Data_Registros.ItemsSource = Lista;
                    }
                }
            }
        }

        private void btn_RegistrosE_Click(object sender, RoutedEventArgs e)
        {
            // 04 leer
            if (clie != null || clieTLS != null)
            {
                byte[] peticion;

                peticion = Func.cuartafuncion();

                byte[] respuesta = new byte[256];
                bool err;
                int res = 0;
                if (clie != null)
                    res = clie.enviaDatos(peticion, peticion.Length);
                if (clieTLS != null)
                    res = clieTLS.enviaDatos(peticion, peticion.Length);
                if (res == peticion.Length)
                {
                    if (clie != null)
                        res = clie.recibeDatos(respuesta, respuesta.Length);
                    if (clieTLS != null)
                        res = clieTLS.recibeDatos(respuesta, respuesta.Length);
                    err = Error.errorfunc(respuesta, res);
                    if (err == true)
                    {
                        List<dataGrid> Lista = new List<dataGrid>();
                        dataGrid elemento;
                        int k = Convert.ToInt32(tb_PrimerRegistroE.Text);
                        byte[] temp1 = new byte[2];
                        int maxBits;
                            for (int j = 0; j < Convert.ToUInt16(tb_NumRegistrosE.Text); j++)
                            {
                                elemento = new dataGrid();
                                elemento.nSalida = k++;
                            Array.Copy(respuesta, 10+2*j, temp1, 0, 2);
                            //Array.Reverse(temp1);
                            elemento.estadoR = BitConverter.ToInt16(temp1);
                                Lista.Add(elemento);

                            }
                        Data_RegistrosE.ItemsSource = Lista;
                    }

                }
            }

        }

        private void btn_PedirError_Click(object sender, RoutedEventArgs e)
        {
            // Funcion 07, 11 y 12
            if (clie != null || clieTLS != null)
            {
                byte[] peticion;

                peticion = Func.septimafuncion();

                byte[] respuesta = new byte[256];
                bool err;
                int res = 0;
                if (clie != null)
                    res = clie.enviaDatos(peticion, peticion.Length);
                if (clieTLS != null)
                    res = clieTLS.enviaDatos(peticion, peticion.Length);
                if (res == peticion.Length)
                {
                    if (clie != null)
                        res = clie.recibeDatos(respuesta, respuesta.Length);
                    if (clieTLS != null)
                        res = clieTLS.recibeDatos(respuesta, respuesta.Length);
                    err = Error.errorfunc(respuesta, res);
                    if (err == true)
                    {

                        tb_Eventos.AppendText("Error desde llamada ="+ Convert.ToUInt16(respuesta[8]) + "\n");
                    }

                }
                peticion = Func.undecimafuncion();
                if (clie != null)
                    res = clie.enviaDatos(peticion, peticion.Length);
                if (clieTLS != null)
                    res = clieTLS.enviaDatos(peticion, peticion.Length);
                if (res == peticion.Length)
                {
                    if (clie != null)
                        res = clie.recibeDatos(respuesta, respuesta.Length);
                    if (clieTLS != null)
                        res = clieTLS.recibeDatos(respuesta, respuesta.Length);
                    err = Error.errorfunc(respuesta, res);
                    if (err == true)
                    {
                        byte[] temp1 = new byte[2];
                        tb_Eventos.AppendText("Evento de la maquina" + Convert.ToUInt16(respuesta[8]) +"\n");
                        Array.Copy(respuesta, 10, temp1, 0, 2);
                        tb_NumeroMensajes.Text = Encoding.Default.GetString(temp1);
                    }

                }
                peticion = Func.duodecimafuncion();
                if (clie != null)
                    res = clie.enviaDatos(peticion, peticion.Length);
                if (clieTLS != null)
                    res = clieTLS.enviaDatos(peticion, peticion.Length);
                if (res == peticion.Length)
                {
                    if (clie != null)
                        res = clie.recibeDatos(respuesta, respuesta.Length);
                    if (clieTLS != null)
                        res = clieTLS.recibeDatos(respuesta, respuesta.Length);
                    err = Error.errorfunc(respuesta, res);
                    if (err == true)
                    {
                        byte[] temp1 = new byte[2];
                        tb_Eventos.AppendText("Evento de la maquina" + Convert.ToUInt16(respuesta[9]) + "\n");
                        Convert.ToUInt16(respuesta[11]);
                        Array.Copy(respuesta,11, temp1, 0, 2);
                        tb_NumeroEvento.Text = Encoding.Default.GetString(temp1);
                        Array.Copy(respuesta, 13, temp1, 0, 2);
                        tb_NumeroMensajes.Text = Encoding.Default.GetString(temp1);

                        for (i=0;i< respuesta[8]; i++)
                        {

                            tb_Eventos.AppendText("Evento" + i + Encoding.UTF8.GetString(respuesta, 15+i, 1) + "\n");
                        }
                    }

                }
            }

        }
    }
}
