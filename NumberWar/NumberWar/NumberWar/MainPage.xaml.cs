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
    enum EnumVista { Soldados, Grilla, Inicio, Help, Ganaste };

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

        private Boolean SoldadosAzules = true;


        public MainPage()
        {
            InitializeComponent();

            
            
            GrillaT = new GrillaWN(Grilla, Color.Aquamarine);
            GrillaT.Dimension_C = DIMENSION_C;
            GrillaT.Dimension_R = DIMENSION_F;

            GrillaT.ArmarGrilla();
            GrillaT.evMensaje += GrillaT_evMensaje;
            
            ListaNuevos.SelectionMode = ListViewSelectionMode.Single;
            SetupListView();

            
            Vista(EnumVista.Inicio);
            GrillaT.LoadConfig();
            RecuperarUltimoJuego();
        }

        private void GrillaT_evMensaje(object sender, MensajeEventArgs e)
        {
            if (e._Mensaje == "Ganaste!!!")
            {
                Vista(EnumVista.Ganaste);
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

        private void RecuperarUltimoJuego()
        { 
            try
            {
                if (GrillaT.ExistsConfig("SoldadosAzules"))
                {
                    if (GrillaT.GetConfig("SoldadosAzules") == "SI")
                    {
                        GrillaT.ColorMisVectores = Color.Blue;
                        GrillaT.ColorWM_Vectores = Color.Red;
                        GrillaT.ActualSoldadosAzules = true;
                        SoldadosAzules = true;
                    }
                    else
                    {
                        GrillaT.ActualSoldadosAzules = false;
                        SoldadosAzules = false;
                        GrillaT.ColorMisVectores = Color.Red;
                        GrillaT.ColorWM_Vectores = Color.Blue;
                    }

                    btnCambio.BackgroundColor = SoldadosAzules? Color.Blue : Color.Red;

                    GrillaT.RecuperarDeArchivos();
                    Vista(EnumVista.Grilla);
                    GrillaT.Mostrar();
                    Coordenadas2.Text = GrillaT.MensajeValorTotal();
                }
            }
            catch (Exception Ex)
            {
                Coordenadas2.Text = Ex.Message;
            }
        }


        private void btnReset_Clicked(object sender, EventArgs e)
        {
            try
            {
                GrillaT.ActualSoldadosAzules = SoldadosAzules;
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
                
                Vista(EnumVista.Grilla);
                GrillaT.Mostrar();
                Coordenadas2.Text = GrillaT.MensajeValorTotal();
                GrillaT.GuardarEnArchivos();
            }
            catch (Exception Ex)
            {
                Coordenadas2.Text = Ex.Message;
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

                    lblMensaje.FontSize = 18;
                    lblMensaje.Text = "<<               INICIO                >>";
                    GrillaPrincipal.Children.Add(bxvMensaje, 0, 1);
                    bxvMensaje.IsVisible = true;

                    break;

                case EnumVista.Ganaste:

                    lblMensaje.FontSize = 18;
                    lblMensaje.Text = "<<               GANASTE                >>";
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

                    String S = @"Objetivo: Tapar las celdas rojas con los Soldados. 
                        
Soldados: Son las listas de 4 elementos azules.
                    
El botón Soldados muestra la lista de soldados, hago click en uno, se cierra la lista.
Hago click en una celda de la grilla, pone el Soldado en esa celda. Los 4 elementos del Soldado están apilados en la misma celda.

Empiezo a arrastrar el soldado para ver donde me conviene dejarlo. Se arrastra desde la cabeza (la celda que tiene el *). NO se puede arrastrar en diagonal o marcha atrás.

Un elemento de un Soldado tapa un enemigo si su valor es mayor.(El valor es el nro de la izquierda de la celda)

Podemos:
Apilar elementos de un Soldado.Si arrastro la celda 3 sobre la 2 de un Soldado, su valor se suma. 
Cambiar el orden de los elementos de un soldado: Doble click en una celda, la intercambia con la que tiene adelante.
Seleccionar un Soldado que esta abajo de otro. Mantengo presionada la celda por 5 seg y lo pone arriba.";

                    lblMensaje.FontSize = 10;
                    lblMensaje.Text = S;
                    GrillaPrincipal.Children.Add(bxvMensaje, 0, 1);
                    bxvMensaje.IsVisible = true;

                    break;

            }
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

        private void btnSoldados_Clicked(object sender, EventArgs e)
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

        private void btnCerrar_Nuevos_Clicked(object sender, EventArgs e)
        {
            Vista(EnumVista.Grilla);
        }

        private void btnCerrar_Mensajes_Clicked(object sender, EventArgs e)
        {
            Vista(EnumVista.Grilla);
        }
    }
}
