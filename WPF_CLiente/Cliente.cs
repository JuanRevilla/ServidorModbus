using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WPF_CLiente
{
    public class Cliente
    {
        private delegadoTexto banner;
        public Thread hiloConectado;
        private Socket clienteTCP = null;
        private IPEndPoint ipEP = null;
        public bool online = false;        
    public Cliente(String dirIP, delegadoTexto _banner) 
        {
            try
            {
                banner = _banner;
                IPAddress dir = IPAddress.Parse(dirIP);
                ipEP = new IPEndPoint(dir, 502);
                clienteTCP = new Socket(dir.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                clienteTCP.SendTimeout = 1000;
                clienteTCP.ReceiveTimeout = 1000;
                hiloConectado = new Thread(new ThreadStart(this.conectarAlServidor));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicial la conexion\n" + ex.ToString(), "Error cliente TCP");
                throw;

            }
        }

        public void conectarAlServidor()
        {
            try
            {
                clienteTCP.Connect(ipEP);
            }
            catch (SocketException ex_sock)
            {
                if(ex_sock.SocketErrorCode == SocketError.ConnectionRefused)
                {
                MessageBox.Show("Error con la ip o el puerto",
                    "Error cliente TCP"+ ipEP.Address.ToString()+ "con puerto" + ipEP.Port.ToString());

                }
                else
                {

                    MessageBox.Show("Error al conectarse al servidor\n" + ex_sock.ToString(),
                        "Error cliente TCP" );

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectarse al servidor\n" + ex.ToString(), "Error cliente TCP");
                throw;
            }
            finally
            {
                if (clienteTCP != null && clienteTCP.Connected)
                {
                    online = true;
                    banner("Cliente conectado");
                }
                else { online = false; banner(""); }
                hiloConectado.Interrupt();
            }

        }
        public void cierraCliente()
        {
            if(clienteTCP != null && online)
            {
                clienteTCP.Close();
            }
            clienteTCP = null;
            online=false;
            try
            {
                if (!hiloConectado.Join(1000)) hiloConectado.Interrupt();
            }
            catch (ThreadInterruptedException ex_hilo)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cerrar el hilo\n" + ex.ToString(),
                        "Error cliente TCP");
                throw;
            }
            return;
        }
        public int enviaDatos(byte[] datos, int dim)
        { try
            {
                if (online)
                {
                    int res = clienteTCP.Send(datos, dim, SocketFlags.None);
                    if (res == dim)
                        return (res);
                    else if (res == 0)
                    {
                        //cierraCliente();
                        return (-1);
                    }
                    else
                    {
                        return (-2);
                    }
                }
                else
                    return (-1);
            }
            catch (SocketException ex_sock) {
                if (ex_sock.SocketErrorCode == SocketError.TimedOut)
                    return (0);
                else if (ex_sock.SocketErrorCode == SocketError.ConnectionReset)
                {
                    cierraCliente();
                    return (-1);
                }
                else
                    return (-2);
            }
            catch(Exception ex) { return (-2); }

                
        }

        public int recibeDatos(byte[] datos, int dimMAX)
        {
            try
            {
                if (online)
                {
                    int res = clienteTCP.Receive(datos, dimMAX, SocketFlags.None);
                    if (res > 0)
                        return (res);
                    else if (res == 0)
                    {
                        //Considera quitar esta linea
                        //clienteTCP = null;
                        //online = false;
                        return (-1);
                    }
                    else
                    {
                        return (-2);
                    }
                }
                else
                    return (-1);
            }
            catch (SocketException ex_sock)
            {
                if (ex_sock.SocketErrorCode == SocketError.TimedOut)
                    return (0);
                else if (ex_sock.SocketErrorCode == SocketError.ConnectionReset)
                {
                    cierraCliente();
                    return (-1);
                }
                else
                    return (-2);
            }
            catch (Exception ex) { return (-2); }     
          }
        
       }
}
