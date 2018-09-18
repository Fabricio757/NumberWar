using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;

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

        public static readonly int CANT_ENEMIGOS = 4;
        public static readonly int CANT_SOLDADOS = 6;

        public static readonly int TAM_ENEMIGO = 3;
        public static readonly int TAM_SOLDADO = 4;


        public GrillaWN GrillaT;


        public MainPage()
        {
            InitializeComponent();
                        
            GrillaT = new GrillaWN(Grilla, Color.Aquamarine);
            GrillaT.Dimension_C = DIMENSION_C;
            GrillaT.Dimension_R = DIMENSION_F;

            GrillaT.ColorMisVectores = Color.Blue;
            GrillaT.ColorWM_Vectores = Color.Red;
            GrillaT.Color_vectorNuevo = Color.WhiteSmoke;

            GrillaT.ArmarGrilla();

            GrillaT.evMensaje += GrillaT_evMensaje;
                       

            ListaNuevos.SelectionMode = ListViewSelectionMode.Single;
            SetupListView();

            Vista(EnumVista.Inicio);

        }

        private void GrillaT_evMensaje(object sender, MensajeEventArgs e)
        {
            if (e._Mensaje == "You Win!!!")
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
                lblInfo.Text = "Total Value Soldiers in List: " + GrillaT.VectoresNuevos.ValorTotal().ToString();
        }

        public void Generar_vectoresEnemigos()
        {
            GrillaT.WN_Vectores.Clear();
            GrillaT.WN_Vectores.GenerarVectores(CANT_ENEMIGOS, TAM_ENEMIGO, GrillaT, false);
            //int I = Convert.ToInt32(txtIngreso.Text);
            //GrillaT.WN_Vectores.GenerarVectores(I, TAM_ENEMIGO, GrillaT, false);

            foreach (Vector v in GrillaT.WN_Vectores)
                foreach (Tiles T in v)
                    T.Valor = T.Valor * -1;
        }

        private void btnReset_Clicked(object sender, EventArgs e)
        {
            GrillaT.tileSeleccionado = null;
            GrillaT.vectorSeleccionado = null;
            GrillaT.vectorNuevo = null;

            GrillaT.MisVectores.Clear();
            GrillaT.WN_Vectores.Clear();
            GrillaT.vectorNuevo = null;
            RandomizeFHA.Reset();
            this.Generar_vectoresEnemigos();
            GrillaT.LimpiarGrilla();
            GrillaT.Mostrar();
            
            GrillaT.VectoresNuevos = new ListaVectores(GrillaT);
            GrillaT.VectoresNuevos.GenerarVectores(CANT_SOLDADOS, TAM_SOLDADO, GrillaT, true);
            GrillaT.ValorTotalVectoresNuevos = GrillaT.VectoresNuevos.ValorTotal();

            Vista(EnumVista.Grilla);

            //Coordenadas2.Text = "Nuevo: " + GrillaT.WN_Vectores.Count().ToString() + " Seed: " + RandomizeFHA.GetSeed().ToString();
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
                this.DisplayAlert("Message", "Soldiers 0", "Ok");
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

            //var documents = "C:\\AUXILIAR";
            /*var documents = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filename = Path.Combine(documents, "Write.txt");
            File.WriteAllText(filename, "Write this text into a file");

            Application.Current.Properties["Nivel"] = "0";*/



            GrillaT.Solucion();
            this.DisplayAlert("Resultado", GrillaT.listaSoluciones.ToString(), "Ok");
            //Vista(EnumVista.Help);

        }



        private void Vista(EnumVista Vista)
        {
            switch (Vista)
            {
                case EnumVista.Inicio:

                    lblMensaje.Text = "-- Begin!! --";
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

                    lblMensaje.Text = "-- Help --";
                    GrillaPrincipal.Children.Add(bxvMensaje, 0, 1);
                    bxvMensaje.IsVisible = true;

                    break;

            }
        }

        private void btnCerrar_Clicked(object sender, EventArgs e)
        {
            Vista(EnumVista.Grilla);
        }
    }
}
