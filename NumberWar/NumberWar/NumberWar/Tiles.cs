using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using TouchTracking;
using System.Threading;

namespace NumberWar
{
    enum EnumEstadoEvento {Nulo, Click, MoveTo, MoveFrom, DobleClick, ClickLento };

    public struct ValorLabel
    {
        public int ValorRojo;
        public int ValorAzul;
        public Boolean Cabecera;
    }


    public class TilesView : Grid
    {
        protected Label L;

        public String Text {
            get
            {
                return L.Text;
            }
            set
            {
                L.Text = value;
            }
        }

        public virtual Color BackgroundClr
        {
            get
            {
                return L.BackgroundColor;
            }
            set
            {
                L.BackgroundColor = value;
                this.BackgroundColor = value;
            }
        }

        public virtual Color TextColor
        {
            get
            {
                return L.TextColor;
            }
            set
            {
                L.TextColor = value;
            }
        }

        public int Col { set; get; }
        public int Row { set; get; }

        public TilesView()
        {
            L = new Label();
        }

        public Boolean MismaUbicacion(TilesView A)
        {
            return ((A.Col == this.Col) && (A.Row == this.Row));
        }

        public int IndiceGrilla()
        {
            return (Col * MainPage.DIMENSION_F + Row);
        }

        public virtual void Limpiar()
        {

        }
    }


    #region Tiles
    public class Tiles
    {
       
        protected GrillaTiles Grilla;

        public int Col { set; get; }
        public int Row { set; get; }
        public int Valor { set; get; }
        public int Id { set; get; }


        public Tiles(GrillaTiles G)
        {
            Grilla = G;
        }
        
        public Tiles(GrillaTiles G, int _Col, int _Row, int _valor)
        {
            Col = _Col;
            Row = _Row;
            Valor = _valor;
            Grilla = G;
        }


        public virtual Tiles Nuevo(GrillaTiles G)
        {
            return (new Tiles(G));
        }

        public virtual Tiles Nuevo(GrillaTiles G, int _Col, int _Row, int _valor)
        {
            Tiles T = new Tiles(G);
            T.Col = _Col;
            T.Row = _Row;
            T.Valor = _valor;
            T.Grilla = G;

            return (T);
        }


        public void CR_RND()
        {
            this.Col = RandomizeFHA.Next(0, MainPage.DIMENSION_C - 1);
            this.Row = RandomizeFHA.Next(0, MainPage.DIMENSION_F - 1);
            this.Valor = RandomizeFHA.Next(1, 8);
        }

        public Tiles VecinoOrtogonal(int Indice)
        {
            int[,] Desplazamientos = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
            Tiles Resultado;

            int IX = this.Col + Desplazamientos[Indice, 0];
            int IY = this.Row + Desplazamientos[Indice, 1];


            if ((IX >= 0) && (IX < Grilla.Dimension_C) && (IY >= 0) && (IY < Grilla.Dimension_R))
                Resultado = this.Nuevo(Grilla, IX, IY, 0);
            else
                Resultado = null;

            return Resultado;
        }

        public Boolean MismaUbicacion(Tiles A)
        {
            return ((A.Col == this.Col) && (A.Row == this.Row));
        }

        public Boolean MismaUbicacion(TilesView A)
        {
            return ((A.Col == this.Col) && (A.Row == this.Row));
        }

        public int IndiceGrilla()
        {
            return (Col * (MainPage.DIMENSION_F - 1) + Row);
        }

        public override String ToString()
        {
            String s = "null string";
            if (this != null)
                s = "(" + this.Col.ToString() + ", " + this.Row.ToString() + " : " + this.Valor.ToString() + ") ";

            return (s);
        }

        public static Tiles copiaDe(GrillaTiles G, Tiles T)
        {
            Tiles RT = G.Tile_RND();

            if (T != null)
            {
                RT.Valor = T.Valor;
                RT.Col = T.Col;
                RT.Row = T.Row;
            }
            else
                RT = null;

            return (RT);
        }


        public virtual void MostrarTileEnGrilla()
        {//Tiles

        }

        public virtual void OcultarEnGrilla(GrillaTiles G, Tiles T_Seleccionado)
        {

        }

        public virtual void MostrarEnColor(Color C)
        {

        }
    }
    #endregion


    #region Vectores
    public class Vector : List<Tiles>
    {
        public int VectorNumber { set; get; }/*Es el numero interno del vector en una lista de Vectores*/

        public void AgregarUnTile(Tiles T)
        {
            this.Add(T);
        }

        public String VectorToSring
        {
            get
            {
                return ToStringTiles();
            }
        }

        protected GrillaTiles Grilla;

        public Boolean Mostrado { set; get; }

        public Vector(GrillaTiles G)
        {
            Mostrado = false;
            Grilla = G;
        }

        public Boolean Interseccion(Vector v)
        {/*Devuelve true si se interseca con el vector v.*/
            foreach (Tiles T1 in this)
                foreach (Tiles T2 in v)
                    if ((T1.Col == T2.Col) && (T1.Row == T2.Row))
                        return true;

            return false;
        }

        public void Generar_RND(int Longitud)
        {/* Genera un Vector de Longitud igual al parametro, pero con valores al Azar*/


            this.Clear();
            Tiles T1 = Grilla.NuevoTile();
            T1.CR_RND();

            T1.Id = 1;
            this.AddTile(T1);

            for (int i = 2; i <= Longitud; i++)
            {
                if (this.HayLugarParaVecinosOrtogonales(T1))
                {
                    Random rand = new Random();
                    int i_rnd = rand.Next(0, 4);
                    if (i_rnd == 4) i_rnd = 3;

                    Tiles Tv = T1.VecinoOrtogonal(i_rnd);

                    while ((Tv == null) || (this.ExisteTile(Tv)))
                    {
                        i_rnd = (i_rnd + 1) % 4;
                        Tv = T1.VecinoOrtogonal(i_rnd);
                    }

                    Tv.Valor = RandomizeFHA.Next(1, 8);

                    Tv.Id = i;
                    this.AddTile(Tv);
                    T1 = Tv;
                }
                else
                    break;
            }
        }

        public Boolean MismoValores(Vector v)
        {/*Retorna true si el Vector v tiene los mismos valores y en el mismo orden que this. NO tiene en cuenta col y row*/
            Boolean R = false;
            int i = 0;
            foreach (Tiles T in this)
                if (T.Valor == v[i].Valor)
                    R = true;

            return (R);
        }

        public Tiles Buscar_Valor(int valor)
        {
            Tiles R = null;

            foreach (Tiles T in this)
                if (T.Valor == valor)
                {
                    R = T;
                }

            return (R);
        }

        public String ToStringTiles()
        {
            String s = "";
            foreach (Tiles T in this)
                s = s + " [" + T.Valor.ToString() + "]";

            s = s + " (" + this.VectorNumber.ToString() + ")";

            return s;
        }

        public override String ToString()
        {
            String s = "";
            foreach (Tiles T in this)
                s = s + " [" + T.ToString() + "] ";

            s = s + " (" + this.VectorNumber.ToString() + ")";

            return s;
        }

        public Boolean ExisteTile(Tiles T)
        {//El parametor T no tiene que existir en el vector. Busca si hay uno con las mismas coordenadas.
            Boolean R = false;
            if (T != null)
                foreach (Tiles T1 in this)
                {
                    if (T1.MismaUbicacion(T)) R = true;
                }
            return R;
        }

        public void SubirTile(int Indice)
        {
            if (Indice > 0)
            {
                Tiles T1 = this[Indice];
                Tiles T2 = this[Indice-1];
                this.RemoveAt(Indice - 1);
                this.Insert(Indice, T2);
                int _Col = T1.Col;
                int _Row = T1.Row;

                T1.Col = T2.Col;
                T1.Row = T2.Row;

                T2.Row = _Row;
                T2.Col = _Col;
            }
        }

        public void MoverTileA(int Index, int pX, int pY)
        {//Mueve los Tiles, desde Index para atras. los parametros pX,pY es donde muevo el primero.

            if (Index < this.Count)
            {
                Tiles T1 = this[Index];
                int oX = T1.Col; //resguardo la posición original
                int oY = T1.Row;

                if (Grilla.MovimientoPermitido(this, Index, pX, pY))
                {
                    this[Index].Col = pX; this[Index].Row = pY;

                    //Muevo la cola
                    if (Index < this.Count - 1)
                    {
                        if ((this[Index + 1].Col != oX) || (this[Index + 1].Row != oY))
                        {
                            this.MoverTileA_cola(Index + 1, oX, oY);
                        }
                    }
                }
            }
        }

        private void MoverTileA_cola(int Index, int pX, int pY)
        {
            if (Index < this.Count)
            {
                Tiles T1 = this[Index];
                int oX = T1.Col; //resguardo la posición original
                int oY = T1.Row;


                this[Index].Col = pX; this[Index].Row = pY;

                if (Index < this.Count - 1)
                {
                    if ((this[Index + 1].Col != oX) || (this[Index + 1].Row != oY))
                    {
                        this.MoverTileA_cola(Index + 1, oX, oY);
                    }
                }
            }
        }

        public void OcultarEnGrilla(GrillaTiles G)
        {
            foreach (Tiles T1 in this)
            {
                T1.OcultarEnGrilla(G, T1);
            }
            Mostrado = false;
        }

        public void Mostrar()
        {
            Vector v = this;
            //v.Reverse();
            Grilla.MostrarVectorEnGrilla(v);
            //v.Reverse(); //La dejo como estaba
            Mostrado = true;
        }

        public Boolean HayLugarParaVecinosOrtogonales(Tiles T)
        {
            Boolean R = false;
            Tiles T_pivot;

            for (int i = 0; i < 4; i++)
            {
                T_pivot = T.VecinoOrtogonal(i);
                if (T_pivot != null)
                    if (!(this.ExisteTile(T_pivot)))
                    {
                        R = true;
                        i = 4;
                    }
            }
            return (R);
        }

        public int BuscarTile(int pCol, int pRow)
        {
            int Index = 0;
            foreach (Tiles T in this)
            {
                if ((T.Col == pCol) && (T.Row == pRow))
                    return Index;
                Index++;
            }
            return -1;
        }

        public int ValorTile(int pCol, int pRow)
        {/*Devuelve la suma de todos los valores que se encuentran en un tile C,R en un vector*/
            int R = 0;
            foreach (Tiles T in this)
            {
                if ((T.Col == pCol) && (T.Row == pRow))
                    R = R + T.Valor;
            }
            return R;
        }

        public void Concatenar(Vector v)
        {
            foreach (Tiles T in v)
            {
                this.Add(T);
            }
        }

        public void AddTile(Tiles T)
        {
            if (T != null)
            {
                this.Add(T);
            }
        }

        public Vector GetTilesByCR(int pCol, int pRow)
        {//Devuelve un vector con los tiles que se encuentran en la posicion pCol, pRow
            Vector V = new Vector(Grilla);
            foreach (Tiles T in this)
            {
                if ((T.Col == pCol) && (T.Row == pRow))
                {
                    V.Add(T);
                }
            }
            return (V);
        }

        public int ValorTotal()
        {
            int R = 0;
            foreach (Tiles T in this)
            {
                R = R + T.Valor;
            }
            return (R);
        }

        public void MostrarEnColor(Color C)
        {
            foreach (Tiles T in this)
                T.MostrarEnColor(C);
        }
    }
    #endregion


    #region Lista Vectores
    public class ListaVectores : List<Vector>
    {
        protected GrillaTiles Grilla;

        public ListaVectores(GrillaTiles G)
        {
            Grilla = G;
        }

        public void GenerarVectores(int cantVectores, int Longitud, GrillaTiles G, Boolean PermiteIntersecados)
        {
            //Random rand = new Random();
            for (var i = 0; i < cantVectores; i++)
            {
                Vector v = new Vector(Grilla);
                v.Generar_RND(Longitud);

                RandomizeFHA.ssSeed = RandomizeFHA.ssSeed + " v" + i.ToString();
                     
                if (PermiteIntersecados == true)
                    this.AgregarVector(v, PermiteIntersecados);
                else
                    while (this.AgregarVector(v, PermiteIntersecados) == false)
                    {                        
                        v.Generar_RND(Longitud);
                        RandomizeFHA.ssSeed = RandomizeFHA.ssSeed + " h" + i.ToString();
                    };
            }
        }

        public Boolean AgregarVector(Vector v, Boolean PermiteIntersecados)
        {/*Retorna true si lo agrego*/
            Boolean bRet = true;

            if (!PermiteIntersecados)
            {
                foreach (Vector v1 in this)
                    if (v1.Interseccion(v))
                        bRet = false;
            }

            v.VectorNumber = this.Count + 1;

            if (bRet == true)
                this.Add(v);

            return (bRet);
        }

        public Vector BuscarVectorByCR(int Col, int Row)
        {//Devuelve el primer Vector que existe en la lista con un Tile en Col, Row

            Vector r = null;
            foreach (Vector v in this)
            {
                if (v.BuscarTile(Col, Row) != -1)
                {
                    r = v;
                    break;
                }

            }
            return (r);
        }

        public ListaVectores VectoresEnCR(int Col, int Row)
        {//Devuelve una lista con los vectores de la lista this, que pasan por Col, Row
            ListaVectores R = new ListaVectores(Grilla);
                foreach (Vector V in this)
                {
                    if (V.BuscarTile(Col, Row) >= 0)
                        R.Add(V);
                }

            return (R);
        }

        public int Existe(Vector V)
        {//Devuelve el primer Vector que existe en la lista con un Tile en Col, Row

            int r = -1;
            foreach (Vector vi in this)
            {
                r ++;
                if (vi == V)
                    break;
            }
            return (r);
        }

        public void MoverAlFondo(Vector V)
        {//Muevo el vector al último lugar de la lista
            if (this.Existe(V) >= 0)
            {
                this.Remove(V);
                this.Add(V);
            }
        }

        public void MoverAlFrente(Vector V)
        {//Muevo el vector al último lugar de la lista
            if (this.Existe(V) >= 0)
            {
                this.Remove(V);
                this.Insert(0, V);
            }
        }

        public Vector GetTilesByCR(int pCol, int pRow)
        {//Devuelve un con los tiles que se encuentran en la posicion pCol, pRow
            Vector VR = new Vector(Grilla);
            foreach (Vector v in this)
            {
                VR.Concatenar(v.GetTilesByCR(pCol, pRow));
            }
            return (VR);
        }

        public int ValorTotal()
        {
            int R = 0;
            foreach (Vector T in this)
            {
                R = R + T.ValorTotal();
            }
            return (R);
        }

        public override String ToString()
        {
            String s = "";
            foreach (Vector V in this)
                s = s + " (" + V.ToString() + ") ";

            return s;
        }
    }
    #endregion


    #region GrillaTiles y TileEventArgs

    public class RandomizeFHA
    {
        static private int Seed = 0;
        static public String ssSeed = "";

        RandomizeFHA()
        { }

        public static int Next(int Desde, int Hasta)
        {
            /*Random rnd = new Random(Seed);
            Seed = rnd.Next(Seed) + 1;
            Seed = Seed % 1000;
            return rnd.Next(Desde, Hasta);*/
            Seed++;
            int R = (Math.Abs((int)DateTime.Now.Ticks)+1) % 1000;
            return ((R % (Hasta - Desde)) + Desde );
        }

        public static void Reset()
        {
            Seed = 0;
            ssSeed = "";
        }

        public static int GetSeed()
        {
            return (Seed);
        }

        public static string Get_ssSeed()
        {
            return (ssSeed);
        }

    }

    public class Timer

    {

        private readonly TimeSpan _timeSpan;

        private readonly Action _callback;




        private static CancellationTokenSource _cancellationTokenSource;




        public Timer(TimeSpan timeSpan, Action callback)

        {

            _timeSpan = timeSpan;

            _callback = callback;

            _cancellationTokenSource = new CancellationTokenSource();

        }

        public void Start()

        {

            CancellationTokenSource cts = _cancellationTokenSource; // safe copy

            Device.StartTimer(_timeSpan, () =>

            {

                if (cts.IsCancellationRequested)

                {

                    return false;

                }

                _callback.Invoke();

                return true; //true to continuous, false to single use

            });

        }




        public void Stop()

        {

            Interlocked.Exchange(ref _cancellationTokenSource, new CancellationTokenSource()).Cancel();
        }
    }

    public class TileEventArgs : TouchActionEventArgs
    {

        public TilesView Tile { set; get; }
        public int MovioA_Col { set; get; }
        public int MovioA_Row { set; get; }

        public TileEventArgs(long id, TouchActionType type, Point location, bool isInContact) : base(id, type, location, isInContact)
        { }
    }

    public class MensajeEventArgs : TouchActionEventArgs
    {

        public String _Mensaje { set; get; }
        public int _Estado { set; get; }

        public MensajeEventArgs(long id, TouchActionType type, Point location, bool isInContact, string Mensaje, int Estado) : base(id, type, location, isInContact)
        {
            _Mensaje = Mensaje;
            _Estado = Estado;
        }
    }

    public class GrillaTiles : Grid
    {
        public Grid Grilla { set; get; }
        public Color ColorCelda { set; get; }

        public int Dimension_C { set; get; }
        public int Dimension_R { set; get; }

        public TilesView TileView_Seleccionado { set; get; }


        public event EventHandler<TileEventArgs> Tile_Click;
        public event EventHandler<TileEventArgs> Tile_DobleClick;
        public event EventHandler<TileEventArgs> Tile_ClickLento;
        public event EventHandler<TileEventArgs> Tile_MoveFrom;
        public event EventHandler<TileEventArgs> Tile_MoveTo;
        public event EventHandler<MensajeEventArgs> evMensaje;


        private EnumEstadoEvento EventStatus = EnumEstadoEvento.Nulo;
        public Boolean Click_largo = false;
        private Timer T;
        private Timer T_DC;

        public GrillaTiles(Grid G, Color pColorCelda)
        {
            Grilla = G;
            ColorCelda = pColorCelda;

            TouchEffect touchEffect = new TouchEffect();
            touchEffect.Capture = true;
            touchEffect.TouchAction += OnTouchEffectAction;
            G.Effects.Add(touchEffect);

            T = new Timer(TimeSpan.FromSeconds(5), null);
            T_DC = new Timer(TimeSpan.FromSeconds(5), null);
        }

        public void execMensaje(object sender, TouchActionEventArgs args, string Mensaje, int Estado)
        {
            MensajeEventArgs tArgs = new MensajeEventArgs(args.Id, args.Type, args.Location, args.IsInContact, Mensaje, Estado);
            EventHandler< MensajeEventArgs> handler = evMensaje;
            if (handler != null)
            {   
                handler(this, tArgs);
            }
        }

        void ClickLento_TimeOut(object sender, TouchActionEventArgs args, string Mensaje)
        {
            T.Stop(); T_DC.Stop();
            TileEventArgs tArgs = new TileEventArgs(args.Id, args.Type, args.Location, args.IsInContact);
            tArgs.Tile = TileView_Seleccionado;

            EventHandler<TileEventArgs>  handler = Tile_ClickLento;
            if (handler != null)
            {
                handler(this, tArgs);
            }
            //Click_largo = true;
            EventStatus = EnumEstadoEvento.ClickLento;
        }

        void Click_TimeOut(object sender, TouchActionEventArgs args, string Mensaje)
        {
            T.Stop(); T_DC.Stop();
            TileEventArgs tArgs = new TileEventArgs(args.Id, args.Type, args.Location, args.IsInContact);
            tArgs.Tile = TileView_Seleccionado;

            EventHandler<TileEventArgs> handler = Tile_Click;
            if (handler != null)
            {
                handler(this, tArgs);
            }
            EventStatus = EnumEstadoEvento.Nulo;            
        }

        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            EventHandler<TileEventArgs> handler;
            switch (args.Type)
            {               
                case TouchActionType.Pressed:
                    T.Stop();
                    TileView_Seleccionado = (TilesView)this.GetTilesViewByXY(args.Location.X, args.Location.Y);
                    if (TileView_Seleccionado != null)
                    {

                        T = new Timer(TimeSpan.FromSeconds(5), () => ClickLento_TimeOut(sender, args, "TOut "));
                        T.Start();

                        /* Esta parte genera el eveno MoveFrom */
                        TileEventArgs tArgs = new TileEventArgs(args.Id, args.Type, args.Location, args.IsInContact);
                        tArgs.Tile = TileView_Seleccionado;


                        if (EventStatus != EnumEstadoEvento.Click)
                        {
                            handler = Tile_MoveFrom;
                            if (handler != null)
                            {
                                EventStatus = EnumEstadoEvento.MoveFrom;
                                handler(this, tArgs);
                            }
                        }
                        else
                            EventStatus = EnumEstadoEvento.DobleClick;
                    }
                    
                    break;

                case TouchActionType.Moved:
                    if (TileView_Seleccionado != null)
                    {
                        TilesView cuadro_Actual = this.GetTilesViewByXY(args.Location.X, args.Location.Y);

                        if ((cuadro_Actual.Col != -1) || (cuadro_Actual.Row != -1))
                        {
                            if ((!TileView_Seleccionado.MismaUbicacion(cuadro_Actual)) && ((EventStatus == EnumEstadoEvento.MoveTo) || (EventStatus == EnumEstadoEvento.MoveFrom)))
                            {
                                T.Stop();
                                TileEventArgs tArgs = new TileEventArgs(args.Id, args.Type, args.Location, args.IsInContact);
                                tArgs.Tile = TileView_Seleccionado;
                                tArgs.MovioA_Col = cuadro_Actual.Col;
                                tArgs.MovioA_Row = cuadro_Actual.Row;

                                TileView_Seleccionado = cuadro_Actual;
                                EventStatus = EnumEstadoEvento.MoveTo;

                                handler = Tile_MoveTo;
                                if (handler != null)
                                {
                                    handler(this, tArgs);
                                }
                            }
                        }
                    }

                    break;

                case TouchActionType.Released:
                    if (TileView_Seleccionado != null)
                    {
                        T.Stop(); T_DC.Stop();

                        if (EventStatus == EnumEstadoEvento.MoveFrom)
                        {
                            T_DC = new Timer(TimeSpan.FromSeconds(1), () => Click_TimeOut(sender, args, "TOut "));
                            T_DC.Start();
                            EventStatus = EnumEstadoEvento.Click;
                        }

                        if (EventStatus == EnumEstadoEvento.DobleClick)
                        {
                            handler = Tile_DobleClick;
                            if (handler != null)
                            {
                                TileEventArgs tArgs = new TileEventArgs(args.Id, args.Type, args.Location, args.IsInContact);
                                tArgs.Tile = TileView_Seleccionado;

                                EventStatus = EnumEstadoEvento.Nulo;
                                handler(this, tArgs);
                            }
                        }

                        if (EventStatus == EnumEstadoEvento.MoveTo)
                        {
                            TileView_Seleccionado = null;
                            EventStatus = EnumEstadoEvento.Nulo;
                        }
                    }

                    break;
            }
        }

        public TilesView GetTilesViewByXY(Double coordX, Double coordY)
        {
            TilesView LG = new TilesView();

            if ((coordX < Grilla.Width) && (coordX > 0) && (coordY < Grilla.Height) && (coordY > 0))
            {
                double cuadro_width = Grilla.Width / this.Dimension_C;
                double cuadro_height = Grilla.Height / this.Dimension_R;

                int LG_C = (int)(coordX / cuadro_width);
                int LG_R = (int)(coordY / cuadro_height);

                LG = (TilesView)Grilla.Children[LG_C * this.Dimension_R + LG_R];
            }
            else
            {
                LG.Col = -1;
                LG.Row = -1;
            }
            return (LG);
        }

        public TilesView GetTilesViewByCR(int Col, int Row)
        {
            TilesView LG = new TilesView();

            LG = (TilesView)Grilla.Children[Col * this.Dimension_R + Row];

            return (LG);
        }

        public virtual void ArmarGrilla()
        {
            for (var i = 0; i < this.Dimension_C; i++)
            {
                Grilla.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(this.Dimension_C, GridUnitType.Star) });
            }

            for (var i = 0; i < this.Dimension_R; i++)
            {
                Grilla.RowDefinitions.Add(new RowDefinition { Height = new GridLength(this.Dimension_R, GridUnitType.Star) });
            }
        }

        public virtual Tiles NuevoTile()
        {
            return (new Tiles(this));
        }

        public virtual Tiles Tile_RND()
        {
            return (null);
        }

        public virtual void Mostrar()
        {
            //LimpiarGrilla();
        }

        public virtual void MostrarTileEnGrilla(Tiles T)
        {//Hay que reescribirlo si o si

        }

        public void MostrarVectorEnGrilla(Vector V)
        {
            int i = 0;
            foreach (Tiles T in V)
            {
                T.MostrarTileEnGrilla();
                i++;
            }
        }

        public void MostrarListaVectoresEnGrilla(ListaVectores LV)
        {
            foreach (Vector v in LV)
                this.MostrarVectorEnGrilla(v);
        }

        public virtual Boolean MovimientoPermitido(Vector vector, int Index, int pX, int pY)
        {
            Boolean R = false;

            //Muevo el del Index
            if (Index > 0) //Si no es el primero
            {
                if ((pX == vector[Index - 1].Col) && (pY == vector[Index - 1].Row))//Si no es el primero, solo puede mover a la posición del anterior. Sino, no hace nada
                {
                    R = true;
                    /*pX = vector[Index].Col;
                    pY = vector[Index].Row;*/
                }
            }
            else //Verificar que el movimiento sea ortogonal y no vuelva para atras
            {
                
                if ((vector[Index].Col == pX) || (vector[Index].Row == pY)) //Si es diagonal
                {
                    if (vector.Count > 1)
                        if ((vector[Index + 1].Col != pX) || (vector[Index + 1].Row != pY))// Verifico que no sea para atrás(sobre el tile anterior)
                            R = true;
                }
            }
            return R;
        }
    }

    #endregion


}
