using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.IO;

namespace NumberWar
{
    public class TilesViewWN : TilesView
    {
        private Label L_A;
        private Label L_R;
        private Label L_Cab;

        public String ValorRojo
        {
            get
            {
                return L_R.Text;
            }
            set
            {
                L_R.Text = value;
            }
        }

        public String ValorAzul
        {
            get
            {
                return L_A.Text;
            }
            set
            {
                L_A.Text = value;
            }
        }

        public override Color BackgroundClr
        {
            get
            {
                return base.BackgroundClr;
            }
            set
            {
                base.BackgroundClr = value;

                L_A.BackgroundColor = value;
                L_R.BackgroundColor = value;
            }
        }

        public override Color TextColor
        {
            get
            {
                return L.TextColor;
            }
            set
            {
                L.TextColor = value;
                L_A.TextColor = value;
                L_R.TextColor = value;
            }
        }

        public Boolean Cabecera
        {
            get { return (L_Cab.Text == "*");  }
            set
            {
                if (value)
                    L_Cab.Text = "*";
                else
                    L_Cab.Text = "";
            }
        }

        public TilesViewWN()
        {
            this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.60, GridUnitType.Star) });
            this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.40, GridUnitType.Star) });

            this.ColumnSpacing = 1;
            this.RowSpacing = 1;

            Grid G1 = new Grid();
            G1.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.70, GridUnitType.Star) });
            G1.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.30, GridUnitType.Star) });

            L = new Label();
            L.BackgroundColor = Color.AntiqueWhite;
            L.FontSize = 12;
            L.VerticalTextAlignment = TextAlignment.Center;
            L.VerticalOptions = LayoutOptions.Fill;
            L.HorizontalOptions = LayoutOptions.Fill;
            G1.Children.Add(L, 0, 0);

            L_Cab = new Label();
            L_Cab.TextColor = Color.AntiqueWhite;
            L_Cab.FontSize = 8;

            G1.Children.Add(L_Cab,0,1);

            this.Children.Add(G1, 0, 0);

            Grid G = new Grid();
            G.RowDefinitions.Add(new RowDefinition());
            G.RowDefinitions.Add(new RowDefinition());

            L_A = new Label();
            L_A.BackgroundColor = Color.AntiqueWhite;
            L_A.VerticalTextAlignment = TextAlignment.Center;
            L_A.Text = "0";
            L_A.FontSize = 9;
            G.Children.Add(L_A, 0, 0);

            L_R = new Label();
            L_R.BackgroundColor = Color.AntiqueWhite;
            L_R.VerticalTextAlignment = TextAlignment.Center;
            L_R.Text = "0";
            L_R.FontSize = 9;
            L_R.HorizontalOptions = LayoutOptions.FillAndExpand;
            G.Children.Add(L_R, 0, 1);

            G.RowSpacing = 1;

            this.Children.Add(G, 1, 0);
        }

        public override void Limpiar()
        {
            this.BackgroundClr = Color.Aquamarine;
            this.TextColor = Color.Black;
            this.Text = "";
            this.ValorAzul = "0";
            this.ValorRojo = "0";
        }
    }

    public class TilesWN : Tiles
    {
        public TilesWN(GrillaTiles G):base(G)
        {

        }

        [JsonConstructor]
        public TilesWN(int _Col, int _Row, int _valor) : base(_Col, _Row, _valor)
        { }

        public TilesWN(GrillaTiles G, int _Col, int _Row, int _valor) : base(G, _Col, _Row, _valor)
        { }

        public override Tiles Nuevo(GrillaTiles G)
        {
            return (new TilesWN(G));
        }

        public override Tiles Nuevo(GrillaTiles G, int _Col, int _Row, int _valor)
        {
            TilesWN T = new TilesWN(G);
            T.Col = _Col;
            T.Row = _Row;
            T.Valor = _valor;
            T.Grilla = G;

            return (T);
        }


        public override void MostrarTileEnGrilla()
        {//Tiles
            Color BC = Grilla.ColorCelda;

            ValorLabel VL = ((GrillaWN)Grilla).GetValorLabel(this);
            int v_Azul = VL.ValorAzul;
            int v_Rojo = VL.ValorRojo;


            Boolean bAzules = ((GrillaWN)Grilla).ActualSoldadosAzules;

            if ((bAzules && (v_Azul + v_Rojo <= 0)) || (! bAzules && (v_Azul + v_Rojo < 0)))
            {
                BC = ((GrillaWN)Grilla).ColorWM_Vectores;
                if (v_Azul != 0)
                    BC = BC.AddLuminosity(0.17);
            }
            else
            {
                BC = ((GrillaWN)Grilla).ColorMisVectores;
                if (v_Rojo != 0)
                    BC = BC.AddLuminosity(0.15);
            }

            MostrarEnGrillaView(((GrillaWN)Grilla), BC, this.Col, this.Row, v_Azul, v_Rojo, v_Azul + v_Rojo, VL.Cabecera);

        }
        
        public override void OcultarEnGrilla(GrillaTiles G, Tiles T_Seleccionado)
        {
            Color BC = G.ColorCelda;

            ValorLabel VL = ((GrillaWN)G).GetValorLabel(this);
            int v_Azul = VL.ValorAzul;
            int v_Rojo = VL.ValorRojo;

            int suma = v_Azul + v_Rojo - T_Seleccionado.Valor;

            //if (((v_Azul == T_Seleccionado.Valor) && (v_Rojo == 0)) || ((v_Rojo == T_Seleccionado.Valor) && (v_Azul == 0)))
            if((v_Azul - T_Seleccionado.Valor == 0) && (v_Rojo == 0))
                BC = G.ColorCelda;
            else
            {
                Boolean bAzules = ((GrillaWN)Grilla).ActualSoldadosAzules;

                if (suma <= (bAzules ? 0: -1))  
                {
                    BC = ((GrillaWN)G).ColorWM_Vectores;
                    if (v_Azul - T_Seleccionado.Valor != 0)
                        BC = BC.AddLuminosity(0.17);
                }
                else
                {
                    BC = ((GrillaWN)G).ColorMisVectores;
                    //if (v_Rojo - T_Seleccionado.Valor > 0)
                    if (v_Rojo != 0)
                        BC = BC.AddLuminosity(0.15);
                }
             };

            MostrarEnGrillaView(((GrillaWN)G), BC, this.Col, this.Row, v_Azul - T_Seleccionado.Valor, v_Rojo, suma, VL.Cabecera);
        }

        public override void MostrarEnColor(Color C)
        {
            ValorLabel VL = ((GrillaWN)Grilla).GetValorLabel(this);
            int v_Azul = VL.ValorAzul;
            int v_Rojo = VL.ValorRojo;

            MostrarEnGrillaView(((GrillaWN)Grilla), C, this.Col, this.Row, v_Azul, v_Rojo, v_Azul + v_Rojo, VL.Cabecera);
        }


        private void MostrarEnGrillaView(GrillaWN G, Color BC, int Col, int Row, int vAzul, int vRojo, int vTotal, Boolean Cabecera)
        {
            TilesViewWN LG = (TilesViewWN)G.GetTilesViewByCR(Col, Row);
            LG.Text = Math.Abs(vTotal).ToString();
            LG.ValorAzul = vAzul.ToString();
            LG.ValorRojo = Math.Abs(vRojo).ToString();
            LG.BackgroundClr = BC;
            LG.Cabecera = Cabecera;

            if (BC == G.ColorCelda)
                LG.TextColor = G.ColorCelda;
            else
                LG.TextColor = Color.WhiteSmoke;
        }
    }

    public class GrillaWN : GrillaTiles
    {
        //private Tiles T_Aux;
        private Double ActualRatio;
        private Double BestRatio;
        private Dictionary<string, string> Configuracion = new Dictionary<string, string>();

        public Boolean ActualSoldadosAzules { set; get; }

        public ListaVectores MisVectores { set; get; }
        public ListaVectores WN_Vectores { set; get; }

        public Color ColorWM_Vectores { set; get; }
        public Color ColorMisVectores { set; get; }

        public ListaVectores VectoresNuevos;
        public int ValorTotalVectoresNuevos = 0;
        public Vector vectorNuevo;
        public Vector vectorSeleccionado;
        public Tiles tileSeleccionado;

        public GrillaWN(Grid G, Color pColorCelda) : base(G, pColorCelda)
        {
            MisVectores = new ListaVectores(this);
            WN_Vectores = new ListaVectores(this);

            this.Tile_Click += GrillaWN_Tile_Click;
            this.Tile_ClickLento += GrillaWN_Tile_ClickLento;
            this.Tile_MoveFrom += GrillaWN_Tile_MoveFrom;
            this.Tile_MoveTo += GrillaWN_Tile_MoveTo;
            this.Tile_DobleClick += GrillaWN_Tile_DobleClick;
        }
        
        #region Eventos

        private void GrillaWN_Tile_DobleClick(object sender, TileEventArgs e)
        {
            try
            {
                if (tileSeleccionado != null)
                {
                    if (vectorSeleccionado != null)
                    {
                        vectorSeleccionado.OcultarEnGrilla(this);
                        int Indice = vectorSeleccionado.BuscarTile(e.Tile.Col, e.Tile.Row);
                        vectorSeleccionado.SubirTile(Indice);
                        vectorSeleccionado.Mostrar();
                    }
                }
            }
            catch (Exception Ex)
            {
            }
        }

        private void GrillaWN_Tile_MoveFrom(object sender, TileEventArgs e)
        {
            //vectorSeleccionado = null;
            //tileSeleccionado = null;
            //Si hay vector azul, lo selecciono. Y selecciono el tile para moverlo
            vectorSeleccionado = this.MisVectores.BuscarVectorByCR(e.Tile.Col, e.Tile.Row);
            if (vectorSeleccionado != null)
            {
                if (tileSeleccionado != null)
                    tileSeleccionado.MostrarTileEnGrilla();
                    
                int Indice = vectorSeleccionado.BuscarTile(e.Tile.Col, e.Tile.Row);
                tileSeleccionado = vectorSeleccionado[Indice];
                tileSeleccionado.MostrarEnColor(Color.DarkBlue);

                //this.execMensaje(sender, e, s + " Seleccionado despues= " + tileSeleccionado.ToString(), 0);
            }            
        }

        private void GrillaWN_Tile_ClickLento(object sender, TileEventArgs e)
        {
            if (vectorSeleccionado != null)
                if (tileSeleccionado != null)
                {
                    //Vector V = this.MisVectores.BuscarVectorByCR(tileSeleccionado.Col, tileSeleccionado.Row);
                    ListaVectores L = this.MisVectores.VectoresEnCR(tileSeleccionado.Col, tileSeleccionado.Row);
                    L.MoverAlFondo(vectorSeleccionado);                    
                    vectorSeleccionado = L[0];
                    this.MisVectores.MoverAlFrente(vectorSeleccionado);
                    vectorSeleccionado.MostrarEnColor(Color.DarkGoldenrod);
                    int Indice = vectorSeleccionado.BuscarTile(tileSeleccionado.Col, tileSeleccionado.Row);
                    tileSeleccionado = vectorSeleccionado[Indice];
                }
        }

        private void GrillaWN_Tile_Click(object sender, TileEventArgs e)
        {
            try
            {
                if (vectorNuevo != null)
                {
                    if (vectorNuevo.Mostrado == false)
                    {
                        foreach (Tiles T in vectorNuevo)
                        {
                            T.Col = e.Tile.Col;
                            T.Row = e.Tile.Row;
                        }
                        MisVectores.Add(vectorNuevo);
                        VectoresNuevos.Remove(vectorNuevo);
                        
                        vectorNuevo.Mostrar();

                        GuardarEnArchivos_Soldados();
                    }
                    this.execMensaje(sender, e, MensajeValorTotal(), 0);
                }
            }
            catch (Exception Ex)
            {
                this.execMensaje(sender, e, Ex.Message, 0);
            }
        }

        private void GrillaWN_Tile_MoveTo(object sender, TileEventArgs e)
        {
            try
            {
                if (tileSeleccionado != null)
                {
                    if (e.MovioA_Row >= 0)
                    {
                        int Indice = vectorSeleccionado.BuscarTile(tileSeleccionado.Col, tileSeleccionado.Row);
                        vectorSeleccionado.OcultarEnGrilla(this);

                        vectorSeleccionado.MoverTileA(Indice, e.MovioA_Col, e.MovioA_Row);
                        if (vectorSeleccionado != null)
                            vectorSeleccionado.Mostrar();

                        this.execMensaje(sender, e, MensajeValorTotal(), 0);
                    }
                    else
                    {
                        this.execMensaje(sender, e, "Se paso pa arriva", 0);
                    }
                }
            }
            catch (Exception Ex)
            {
                this.execMensaje(sender, e, "No hay Vector seleccionado", 0);
            }
        }

        #endregion

        public void GuardarEnArchivos()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filenameVectoresEnemigos = Path.Combine(documents, "VectoresEnemigos.txt");
            string jsonVectoresEnemigos = JsonConvert.SerializeObject(this.WN_Vectores);
            File.WriteAllText(filenameVectoresEnemigos, jsonVectoresEnemigos);

            var filenameVectoresNuevos = Path.Combine(documents, "VectoresNuevos.txt");
            string jsonVectoresNuevos = JsonConvert.SerializeObject(this.VectoresNuevos);
            File.WriteAllText(filenameVectoresNuevos, jsonVectoresNuevos);

            GuardarEnArchivos_Soldados();
        }

        public void GuardarEnArchivos_Soldados()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filenameVectoresSoldados = Path.Combine(documents, "VectoresSoldados.txt");
            string jsonVectoresSoldados = JsonConvert.SerializeObject(this.MisVectores);
            File.WriteAllText(filenameVectoresSoldados, jsonVectoresSoldados);

            this.SetConfig("SoldadosAzules", ActualSoldadosAzules ? "SI" : "NO");            
        }

        public void RecuperarDeArchivos()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filenameVectoresNuevos = Path.Combine(documents, "VectoresNuevos.txt");
            var filenameVectoresEnemigos = Path.Combine(documents, "VectoresEnemigos.txt");
            var filenameVectoresSoldados = Path.Combine(documents, "VectoresSoldados.txt");

            string jsonVectoresSoldados = File.ReadAllText(filenameVectoresSoldados);
            MisVectores = JsonConvert.DeserializeObject<ListaVectores>(jsonVectoresSoldados);
            MisVectores.SetGrilla(this);
            MisVectores = ConvertirLista(MisVectores);
            MisVectores.RenumerarVectores();

            string jsonVectoresEnemigos = File.ReadAllText(filenameVectoresEnemigos);
            WN_Vectores = JsonConvert.DeserializeObject<ListaVectores>(jsonVectoresEnemigos);
            WN_Vectores.SetGrilla(this);
            WN_Vectores = ConvertirLista(WN_Vectores);
            WN_Vectores.RenumerarVectores();

            string jsonVectoresNuevos = File.ReadAllText(filenameVectoresNuevos);
            VectoresNuevos = JsonConvert.DeserializeObject<ListaVectores>(jsonVectoresNuevos);
            VectoresNuevos.SetGrilla(this);
            VectoresNuevos = ConvertirLista(VectoresNuevos);
            VectoresNuevos.RenumerarVectores();

            ValorTotalVectoresNuevos = VectoresNuevos.ValorTotal();

            VectoresNuevos.Restar(MisVectores);


            RecuperarMejorRazon();
        }

        #region Configuracion

        public String GetConfig(String Clave)
        {
            return Configuracion[Clave];
        }

        public Boolean ExistsConfig(String Clave)
        {
            return Configuracion.ContainsKey(Clave);
        }

        public void LoadConfig()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filename = Path.Combine(documents, "Configuracion.txt");
            if (! File.Exists(filename))
            {
                SetConfig("SoldadosAzules", "SI");
                SetConfig("BestRatioBlue", "1000");
            }
            string jsonS = File.ReadAllText(filename);
            Configuracion = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonS);

        }

        public void SetConfig(String Clave, String Valor)
        {
            if (Configuracion.ContainsKey(Clave))
                Configuracion[Clave] = Valor;
            else
                Configuracion.Add(Clave, Valor);

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filename = Path.Combine(documents, "Configuracion.txt");

            string jsonS = JsonConvert.SerializeObject(Configuracion);
            File.WriteAllText(filename, jsonS);
        }

        #endregion

        private ListaVectores ConvertirLista(ListaVectores L)
        {/* Sisi, perdimos la elegancia....*/
            ListaVectores LR = new ListaVectores(this);

            foreach (Vector V in L)
            {
                Vector VR = new Vector(this);
                foreach (Tiles T in V)
                {
                    TilesWN TW = new TilesWN(this, T.Col, T.Row, T.Valor);
                    VR.Add(TW);
                }
                LR.Add(VR);
            }

            return LR;
        }

        public override Boolean MovimientoPermitido(Vector vector, int Index, int pX, int pY)
        {
            Boolean b = base.MovimientoPermitido(vector, Index, pX, pY);
            if (b == false)
            {
                tileSeleccionado = null;
                //vectorSeleccionado = null;
            }
            return b;
        }

        public ValorLabel GetValorLabel(Tiles T)
        {
            Vector V_Azul = MisVectores.GetTilesByCR(T.Col, T.Row);
            Vector V_Rojos = WN_Vectores.GetTilesByCR(T.Col, T.Row);

            ValorLabel v = new ValorLabel();
            v.ValorAzul = V_Azul.ValorTotal();
            v.ValorRojo = V_Rojos.ValorTotal();

            v.Cabecera = false; 
            foreach (Vector v1 in MisVectores)
            {
                if(v1.BuscarTile(T.Col, T.Row) == 0)
                    v.Cabecera = true;
            }

            return (v);
        }

        public override void ArmarGrilla()
        {
            base.ArmarGrilla();

            for (var i = 0; i < this.Dimension_C; i++)
            {
                for (var j = 0; j < this.Dimension_R; j++)
                {
                    TilesViewWN cuadro = new TilesViewWN();
                    cuadro.Text = (i * this.Dimension_R + j).ToString();
                    cuadro.BackgroundClr = ColorCelda;
                    cuadro.TextColor = ColorCelda;
                    cuadro.Col = i;
                    cuadro.Row = j;

                    //Grilla.Children.Add(cuadro);
                    Grilla.Children.Add(cuadro, i, j);
                }
            }
        }

        public virtual void LimpiarGrilla()
        {
            for (var i = 0; i < this.Dimension_C; i++)
            {
                for (var j = 0; j < this.Dimension_R; j++)
                {
                    TilesViewWN cuadro = (TilesViewWN)Grilla.Children[i * this.Dimension_R + j];
                    cuadro.Text = (i * this.Dimension_R + j).ToString();
                    cuadro.BackgroundClr = ColorCelda;
                    cuadro.TextColor = ColorCelda;
                    cuadro.Col = i;
                    cuadro.Row = j;
                }
            }
        }

        public override void Mostrar()
        {
            base.Mostrar();

            this.MostrarListaVectoresEnGrilla(MisVectores);
            this.MostrarListaVectoresEnGrilla(WN_Vectores);
        }

        public override void MostrarTileEnGrilla(Tiles T)
        {//Hay que reescribirlo si o si
            T.MostrarTileEnGrilla();
        }

        public int ValorTotalTile(int Col, int Row)
        {
            int R = 0;
            int Indice = -1;
            foreach (Vector V in this.WN_Vectores)
            {
                Indice = V.BuscarTile(Col, Row);
                if (Indice > -1)
                    R = R + V[Indice].Valor;
            }
            foreach (Vector V in this.MisVectores)
            {
                R = R + V.ValorTile(Col, Row);
            }

            return R;
        }

        public String MensajeValorTotal()
        {
            String S = "";
            int Neto_Rojo = 0;
            int V_Tile = 0;
            int CuadritosRojos = 0;
            foreach (Vector V in this.WN_Vectores)
                foreach (Tiles T in V)
                {
                    V_Tile = this.ValorTotalTile(T.Col, T.Row);
                    if ((ActualSoldadosAzules && (V_Tile <= 0)) || (! ActualSoldadosAzules && (V_Tile < 0)))
                    {
                        CuadritosRojos++;
                        Neto_Rojo = Neto_Rojo + V_Tile;
                    }
                }

            int valorSoldados = this.VectoresNuevos.ValorTotal();
            int valorMisVectores = this.MisVectores.ValorTotal();
            int valorVectoresEnmigos = this.WN_Vectores.ValorTotal();

            Double TotalRatio = Math.Abs((Double)valorSoldados / (Double)valorVectoresEnmigos);
            ActualRatio = Math.Abs((Double)valorMisVectores / (Double)valorVectoresEnmigos);

            if (CuadritosRojos == 0)
            {
                S = "Ganaste!!!";
                if (ActualRatio < BestRatio)
                {
                    string mm = ActualRatio.ToString("N2");
                    if (ActualSoldadosAzules)
                        SetConfig("BestRatioBlue", mm);
                    else
                        SetConfig("BestRatioRed", mm);
                }
            }
            else
            {
                S = S + "Puntos Enemigos: " + Math.Abs(valorVectoresEnmigos).ToString() + " Puntos Soldados: " + valorSoldados.ToString() + " Soldados/Enemigos: " + TotalRatio.ToString("N2") + "\n";
                S = S + "Enemigos Vivos: " + CuadritosRojos.ToString() + " Puntos: " + Math.Abs(Neto_Rojo).ToString() + '\n';
                S = S + "Puntos Azules Disponibles: " + VectoresNuevos.ValorTotal().ToString() + '\n';
                S = S + "RAZÓN ACTUAL (Puntos de Soldados aplicados/puntos enemigos): " + ActualRatio.ToString("N2") + "   MEJOR RAZÓN: " + BestRatio.ToString("N2");
            };

            return (S);
        }

        public override Tiles NuevoTile()
        {
            return (new TilesWN(this));
        }

        public void NuevoJuego(int Cant_Soldados, int Tam_Soldado, int Cant_Enemigos, int Tam_Enemigo)
        {
            tileSeleccionado = null;
            vectorSeleccionado = null;
            vectorNuevo = null;

            MisVectores.Clear();
            WN_Vectores.Clear();
            
            RandomizeFHA.Reset();

            //Enemigos
            WN_Vectores.Clear();
            WN_Vectores.GenerarVectores(Cant_Enemigos, Tam_Enemigo, this, false);

            foreach (Vector v in WN_Vectores)
                foreach (Tiles T in v)
                    T.Valor = T.Valor * -1;

            LimpiarGrilla();

            Mostrar();

            //Soldados
            VectoresNuevos = new ListaVectores(this);
            VectoresNuevos.GenerarVectores(Cant_Soldados, Tam_Soldado, this, true);
            RecuperarMejorRazon();
        }

        private void RecuperarMejorRazon()
        {
            BestRatio = 1000;
            if (ActualSoldadosAzules)
            {
                if (this.ExistsConfig("BestRatioBlue"))
                    BestRatio = Convert.ToDouble(this.GetConfig("BestRatioBlue"));
            }
            else
            { 
                if (this.ExistsConfig("BestRatioRed"))
                    BestRatio = Convert.ToDouble(this.GetConfig("BestRatioRed"));

            }
        }

        #region Solución

        private void IntercambioValores(Tiles T1, Tiles T2)
        {
            int Valor0 = T1.Valor;
            T1.Valor = T2.Valor;
            T2.Valor = Valor0;
        }

        private void armarMatriz(int[,] Matriz)
        {/* Devuelve la matriz con las posibles posiciones de los vectores solución*/
            int i = 0;
            for (int a = 0; a <= 3; a++)
                for (int b = 0; b <= 3; b++)
                    for (int c = 0; c <= 3; c++)
                    {
                        Matriz[i, 0] = 0; //Este va siempre 0
                        Matriz[i, 1] = a;
                        Matriz[i, 2] = b;
                        Matriz[i, 3] = c;
                        i++;
                    }
        }

        public ListaVectores FactorialValores(Vector V)
        {
            ListaVectores L = new ListaVectores(this);
            if (V.Count == 2)
            {
                L.Add(V.Clon());

                Vector v2 = V.Clon();
                IntercambioValores(v2[0], v2[1]);
                L.Add(v2);
            }
            else
            {
                for (int i = 0; i < V.Count; i++)
                {
                    Vector V2 = V.Clon();
                    V2.RemoveByIndex(i);
                    ListaVectores L2 = FactorialValores(V2);
                    for (int j = 0; j < L2.Count; j++)
                    {
                        L2[j].Insert(0, V[i]);
                        L.Add(L2[j]);
                    }

                }

            }
            return (L);
        }

        int[,] MatrizPocisiones = new int[64, 4];
        int[,] DicColRow = new int[4, 2] { { 0,0}, { 1, 0 }, {0,1 }, {0,-1 }};
        public ListaVectores listaSoluciones;
        int iN = 0;

        public Boolean SolucionR(int Index_Azul, int Nivel)   
        {
            Boolean Retorno = false;

            if (TapeTodo())
                Retorno = true;
            else
            if (Index_Azul < VectoresNuevos.Count)
            {
                Vector v_Azul = this.VectoresNuevos[Index_Azul];

                if (Nivel == 5)
                    Nivel = 5;

                foreach (Vector v_Rojo in this.WN_Vectores)
                {
                    if (Retorno)
                        break;

                    foreach (Tiles T_Rojo in v_Rojo)            //Recorro todos los Tiles Rojos
                    {
                        if (Retorno)
                            break;
                        ListaVectores listaFactorial = FactorialValores(v_Azul);
                        foreach (Vector v_Factorial in listaFactorial)
                        {
                            if (Retorno)
                                break;

                            for (int i = 63; i >= 0; i--)
                            {
                                iN++;
                                v_Factorial[0].Col = T_Rojo.Col;
                                v_Factorial[0].Row = T_Rojo.Row;
                                for (int j = 1; j <= 3; j++)
                                {
                                    v_Factorial[j].Col = v_Factorial[j - 1].Col + DicColRow[MatrizPocisiones[i, j], 0];
                                    v_Factorial[j].Row = v_Factorial[j - 1].Row + DicColRow[MatrizPocisiones[i, j], 1];
                                }
                                Vector Solucion = v_Factorial.Clon();
                                listaSoluciones.Add(Solucion);

                                if (SolucionR(Index_Azul + 1, Nivel + 1))
                                {
                                    Retorno = true;
                                    break; 
                                }
                                else
                                    listaSoluciones.RemoveAt(listaSoluciones.Count - 1); //saco el ultimo

                            }
                        }
                    }
                }
            };
            
            return Retorno;
        }

        private Boolean TapeTodo()
        {
            Boolean Retorno = true;
            
            foreach (Vector V in this.WN_Vectores)
                foreach (Tiles T in V)
                {
                    int TotalAzul = listaSoluciones.GetTilesByCR(T.Col, T.Row).ValorTotal();
                    if (TotalAzul <= Math.Abs(T.Valor))
                    {
                        Retorno = false;
                        break;
                    };
                }

            return Retorno;

        }


        public void Solucion()
        {
            listaSoluciones = new ListaVectores(null);
            armarMatriz(MatrizPocisiones);
            iN = 0;
            SolucionR(0, 0);            
        }

        #endregion
    }

}
