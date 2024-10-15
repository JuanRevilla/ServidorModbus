using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.ComponentModel;

namespace WPF_CLiente
{
    internal class ClienteTLS
    {

        private delegadoTexto banner;
        
        public Thread hiloConectado;
        private Socket clienteTCP = null;
        private IPEndPoint ipEP = null;
        public bool online = false;
        private NetworkStream streamTCP = null;
        private SslStream streamTLS = null;
        private String CertificadoCliente;
        private String CertificadoServidor;
        public ClienteTLS(String dirIP, delegadoTexto _banner, String _CertificadoCliente, String _CertificadoServidor)
        {
            try
            {
                banner = _banner;
                CertificadoCliente = _CertificadoCliente;
                CertificadoServidor = _CertificadoServidor;
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
        public static bool validarCertificadoServidor(object sender, X509Certificate certificadoservidor, X509Chain cadenaCertificacion, SslPolicyErrors errores)
        {
            if (errores == SslPolicyErrors.None)
            {
                return (true);
            }
            else
                MessageBox.Show("Error ccon la verificacion",
                        "Error en el certificado" + errores.ToString());
            return (false);
        }
        public void conectarAlServidor()
        {
            try
            {
                clienteTCP.Connect(ipEP);
                streamTCP = new NetworkStream(clienteTCP, true);
                streamTLS = new SslStream(streamTCP, false, new RemoteCertificateValidationCallback(validarCertificadoServidor), null);
                streamTLS.WriteTimeout = 1000;
                streamTLS.ReadTimeout = 1000;
                X509Certificate2Collection colecionCertificadosCliente = null;
                if (CertificadoCliente != null)
                {
                    X509Store store = new X509Store(StoreLocation.CurrentUser);
                    store.Open(OpenFlags.ReadOnly);
                    X509Certificate2Collection todocertificados = store.Certificates;
                    colecionCertificadosCliente = todocertificados.Find(X509FindType.FindBySubjectDistinguishedName, CertificadoCliente, false);
                    store.Close();
                }
                streamTLS.AuthenticateAsClient(CertificadoServidor, colecionCertificadosCliente, SslProtocols.Tls12, false);
                streamTLS.ReadTimeout = 1000;
            }
            catch (Exception ex) when (ex.InnerException is Win32Exception) 
            {
                if (((Win32Exception)(ex.InnerException)).ErrorCode == 10053 || ((Win32Exception)(ex.InnerException)).ErrorCode == 1054)
                {
                    MessageBox.Show("Servidor Desconectado");
                    cierraCliente();
                }
                else
                    MessageBox.Show("Algo Raro");
                cierraCliente();
               
            }
            catch (AuthenticationException ex_auth)
            {
                MessageBox.Show("Error con la autencificacion" +
                       ex_auth.ToString());
                cierraCliente();
            }
            catch (SocketException ex_sock)
            {
                if (ex_sock.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    MessageBox.Show("Error con la ip o el puerto",
                        "Error cliente TCP" + ipEP.Address.ToString() + "con puerto" + ipEP.Port.ToString());

                }
                else
                {

                    MessageBox.Show("Error al conectarse al servidor\n" + ex_sock.ToString(),
                        "Error cliente TCP");

                }
                cierraCliente();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectarse al servidor\n" + ex.ToString(), "Error cliente TCP");
                cierraCliente();
                throw;
            }
            finally
            {
                if (streamTLS != null && streamTLS.CanRead && streamTLS.CanWrite)
                {
                    online = true;
                    banner("Cliente conectado");
                }
                else
                {
                    online = false;
                    banner("");
                }
                hiloConectado.Interrupt();
            }

        }
        public void cierraCliente()
        {
            if (streamTLS != null && online)
            {
                streamTLS.Close();
            }
            streamTLS = null;
            streamTCP = null;
            clienteTCP = null;
            online = false;
            try
            {
                if (!hiloConectado.Join(1000)) hiloConectado.Interrupt();
            }
            catch (ThreadInterruptedException ex_hilo)
            {
                ;
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
        {
            try
            {
                if (online)
                {
                    streamTLS.Write(datos,0,dim);
                    streamTLS.Flush();
                    return dim;
                }
                else
                    return (-1);
            }
            catch (Exception ex) when (ex.InnerException is Win32Exception)
            {
             if (((Win32Exception) (ex.InnerException)).ErrorCode == 10053 || ((Win32Exception)(ex.InnerException)).ErrorCode == 1054)
                {
                    MessageBox.Show("Servidor Desconectado");
                    cierraCliente();
                    return (-1);
                }
                else
                    MessageBox.Show("Algo Raro");
                return (-2);

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

        public int recibeDatos(byte[] datos, int dimMAX)
        {
            int leidos = 0;
            int timeoutRead = streamTLS.ReadTimeout;
            try
            {
                if (online)
                {
                    int res;
                    while (leidos < dimMAX)
                    {
                        res = streamTLS.Read(datos, leidos, dimMAX - leidos);

                       if (res > 0) { 
                                leidos += res;
                            streamTLS.ReadTimeout = 100;

                        } 
                        else if (res == 0)
                        {
                                //Considera quitar esta linea
                                //clienteTCP = null;
                                //online = false;
                                break;
                        }
                    }
                    return leidos;
                }
                else
                    return (-1);
            }
            catch (ObjectDisposedException ex_Ssl)
            {
                return (-1);
            }
            catch (Exception ex) when (ex.InnerException is Win32Exception)
            {
                if (((Win32Exception)(ex.InnerException)).ErrorCode == 10053 || ((Win32Exception)(ex.InnerException)).ErrorCode == 1054)
                {
                    cierraCliente();
                    return (-1);
                }
                else
                    return (-2);

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
            finally
            {
                streamTLS.ReadTimeout = timeoutRead;
            }
        }
    }
}
