using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;
using Newtonsoft.Json;


using TouchTracking;


namespace NumberWar
{
    enum EnumVista { Soldados, Grilla, Inicio, Help};

    public partial class MainPage : ContentPage
    {/*
        public static readonly int DIMENSION_C = 5;
        public static readonly int DIMENSION_F = 5;

        public static readonly int CANT_ENEMIGOS = 2;
        public static readonly int CANT_SOLDADOS = 2;

        public static readonly int TAM_ENEMIGO = 3;
        public static readonly int TAM_SOLDADO = 4;*/

        public static readonly int DIMENSION_C = 10;
        public static readonly int DIMENSION_F = 10;

        public static readonly int CANT_ENEMIGOS = 10;
        public static readonly int CANT_SOLDADOS = 12;

        public static readonly int TAM_ENEMIGO = 3;
        public static readonly int TAM_SOLDADO = 4;



        public GrillaWN GrillaT;

        private Boolean Inicio = false;
        private Boolean SoldadosAzules = true;
        

        public MainPage()
        {
            InitializeComponent();
                        
            GrillaT = new GrillaWN(Grilla, Color.Aquamarine);
            GrillaT.Dimension_C = DIMENSION_C;
            GrillaT.Dimension_R = DIMENSION_F;

            SoldadosAzules = true;
            GrillaT.ColorMisVectores = Color.Blue;
            GrillaT.ColorWM_Vectores = Color.Red;


            if (Application.Current.Properties.ContainsKey("SoldadosAzules"))
            {
                if (Application.Current.Properties["SoldadosAzules"].ToString() == "NO")
                {
                    SoldadosAzules = false;
                    GrillaT.ColorMisVectores = Color.Red;
                    GrillaT.ColorWM_Vectores = Color.Blue;
                }
            }

            GrillaT.ActualSoldadosAzules = SoldadosAzules;

            GrillaT.ArmarGrilla();
            GrillaT.evMensaje += GrillaT_evMensaje;                       

            ListaNuevos.SelectionMode = ListViewSelectionMode.Single;
            SetupListView();

            Inicio = true;

            Vista(EnumVista.Inicio);

        }

        private void GrillaT_evMensaje(object sender, MensajeEventArgs e)
        {
            if (e._Mensaje == "Ganaste!!!")
            {
                GrillaPrincipal.Children.Add(bxvMensaje, 0, 1);
                lblMensaje.Text = e._Mensaje;

                bxvMensaje.IsVisible = true;
                Grilla.IsVisible = false;
            }

            Coordenadas2.Text = e._Mensaje;
        }

        private void SetupListView()
        {
            var template = new DataTemplate(typeof(TextCell));
            template.SetBinding(TextCell.TextProperty, "VectorToSring");
            ListaNuevos.ItemTemplate = template;
            if (GrillaT.VectoresNuevos != null)
                lblInfo.Text = "Total en la lista: " + GrillaT.VectoresNuevos.ValorTotal().ToString();
        }

        private void btnReset_Clicked(object sender, EventArgs e)
        {

            if (Inicio)
            {
                GrillaT.RecuperarDeArchivos();
                btnCambio.BackgroundColor = SoldadosAzules ? Color.Blue : Color.Red;
                GrillaT.Mostrar();
            }
            else
            {

                if (SoldadosAzules == true)
                {
                    GrillaT.ColorMisVectores = Color.Blue;
                    GrillaT.ColorWM_Vectores = Color.Red;
                    GrillaT.NuevoJuego(CANT_SOLDADOS, TAM_SOLDADO, CANT_ENEMIGOS, TAM_ENEMIGO);
                }
                else
                {
                    GrillaT.ColorMisVectores = Color.Red;
                    GrillaT.ColorWM_Vectores = Color.Blue;
                    GrillaT.NuevoJuego(CANT_ENEMIGOS + 18, TAM_ENEMIGO, CANT_SOLDADOS, TAM_SOLDADO);
                }
                
            }



            GrillaT.ActualSoldadosAzules = SoldadosAzules;
            GrillaT.GuardarEnArchivos();

            Inicio = false;

            Vista(EnumVista.Grilla);
            Coordenadas2.Text = GrillaT.MensajeValorTotal();
        }

        private void btnNuevoVector_Clicked(object sender, EventArgs e)
        {
            try
            {
                SetupListView();
                if (GrillaT.VectoresNuevos.Count == 0)
                    ListaNuevos.ItemsSource = null;
                else
                    ListaNuevos.ItemsSource = GrillaT.VectoresNuevos;

                Vista(EnumVista.Soldados);
            }
            catch (Exception Ex)
            {
                this.DisplayAlert("Mensaje", "Soldado 0", "Ok");
            }
        }


        private void ListaNuevos_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                if (e.Item != null)
                {
                    GrillaT.vectorNuevo = (Vector)e.Item;
                    Vista(EnumVista.Grilla);
                }
                else
                    this.DisplayAlert("Mensaje", "No se seleccionó un Item", "Ok");
            }
            catch (Exception Ex)
            {
                this.DisplayAlert("Error", Ex.Message, "Ok");
            }
        }


        private void btnAux_Clicked(object sender, EventArgs e)
        {
            //this.DisplayAlert("Mensaje", filenameVectoresNuevos, "Ok");
            //GrillaT.Solucion();
            //this.DisplayAlert("Resultado", GrillaT.listaSoluciones.ToString(), "Ok");
            Vista(EnumVista.Help);

        }



        private void Vista(EnumVista Vista)
        {
            switch (Vista)
            {
                case EnumVista.Inicio:

                    lblMensaje.Text = "-- Inicio!! --";
                    GrillaPrincipal.Children.Add(bxvMensaje, 0, 1);
                    bxvMensaje.IsVisible = true;

                    break;

                case EnumVista.Grilla:

                    GrillaPrincipal.Children.Add(Grilla, 0, 1);
                    Grilla.IsVisible = true;
                    bxvMensaje.IsVisible = false;
                    VistaNuevos.IsVisible = false;

                    break;

                case EnumVista.Soldados:

                    GrillaPrincipal.Children.Add(VistaNuevos, 0, 1);
                    VistaNuevos.IsVisible = true;

                    break;

                case EnumVista.Help:

                    lblMensaje.Text = "-- Ayuda --";
                    GrillaPrincipal.Children.Add(bxvMensaje, 0, 1);
                    bxvMensaje.IsVisible = true;

                    break;

            }
        }

        private void btnCerrar_Clicked(object sender, EventArgs e)
        {
            Vista(EnumVista.Grilla);
        }

        private void btnCambio_Clicked(object sender, EventArgs e)
        {
            if (btnCambio.BackgroundColor == Color.Red)
            {
                btnCambio.BackgroundColor = Color.Blue;
                SoldadosAzules = true;
            }
            else
            {
                btnCambio.BackgroundColor = Color.Red;
                SoldadosAzules = false;
            }
        }
    }
}
