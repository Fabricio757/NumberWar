using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using TouchTracking;

namespace NumberWar
{
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
            SetupView();

            lblMensaje.Text = "Inicio";
            GrillaPrincipal.Children.Add(bxvMensaje, 0, 1);
            bxvMensaje.IsVisible=true;

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

        private void SetupView()
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
            
            GrillaPrincipal.Children.Add(Grilla, 0, 1);
            Grilla.IsVisible = true;
            bxvMensaje.IsVisible = false;

            Coordenadas2.Text = "Nuevo: " + GrillaT.WN_Vectores.Count().ToString() + " Seed: " + RandomizeFHA.GetSeed().ToString();
                 
        }

        private void btnNuevoVector_Clicked(object sender, EventArgs e)
        {
            GrillaPrincipal.Children.Add(VistaNuevos, 0, 1);
            SetupView(); 
            ListaNuevos.ItemsSource = GrillaT.VectoresNuevos;
            VistaNuevos.IsVisible = true;
        }

        private void ListaNuevos_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                GrillaT.vectorNuevo = (Vector)e.Item;
                //Coordenadas2.Text = GrillaT.vectorNuevo.ToStringTiles();
                GrillaPrincipal.Children.Add(Grilla, 0, 1);
                VistaNuevos.IsVisible = false;
            }
            else
                this.DisplayAlert("Mensaje", "No se seleccionó un Item", "Ok");
        }

        private void btnAux_Clicked(object sender, EventArgs e)
        {
            Vector vector1 = new Vector(GrillaT);
            Vector vector2 = new Vector(GrillaT);

            vector1.AddTile(new TilesWN(GrillaT, 2, 2, 6));
            vector2.AddTile(new TilesWN(GrillaT, 2, 3, 5));

            GrillaT.MisVectores.Add(vector1);
            GrillaT.MisVectores.Add(vector2);

            Vector vector_Rojo = new Vector(GrillaT);
            vector_Rojo.AddTile(new TilesWN(GrillaT, 0, 0, 6));
            GrillaT.WN_Vectores.Add(vector_Rojo);

            GrillaT.Mostrar();
        }

        private void btnCerrar_Clicked(object sender, EventArgs e)
        {
            GrillaPrincipal.Children.Add(Grilla, 0, 1);
            VistaNuevos.IsVisible = false;
        }
    }
}
