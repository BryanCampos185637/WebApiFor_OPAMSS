using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiFox.Models
{
    public class ConsultaFoxModel
    {
        public string licrespon { get; set; }
        public string codigo_uso { get; set; }
        public string numeroexpediente { get; set; }
        public string estado { get; set; }
        public string descripcionestado { get; set; }
        public string entidad { get; set; }
        public string n_entidad { get; set; }
        public string acceso1 { get; set; }
        public string n_acceso1 { get; set; }
        public string nom_proy { get; set; }
        public string acceso2 { get; set; }
        public string n_acceso2 { get; set; }
        public string acceso3 { get; set; }
        public string n_acceso3 { get; set; }
        public string poligono { get; set; }
        public DateTime f_r1_sal { get; set; }
        public DateTime? f_ing_rec { get; set; }

        DateTime? _f_rno_rec;
        public DateTime? f_rno_rec { get {
                if (_f_rno_rec < f_ing_rec)
                {
                    return f_resoluc;
                }
                else
                {
                    return _f_rno_rec;
                }
            }
            set {
                _f_rno_rec = value;
            }
        }
        public DateTime? f_emi_memo { get; set; }
        public DateTime? f_r1_ing { get; set; }
        public DateTime? f_r2_ing { get; set; }
        public DateTime? f_resoluc  { get; set; }
        public DateTime? retiromemo { get; set; }
        public DateTime? f_sal_rec { get; set; }
        public string municipio { get; set; }
        public DateTime? f_rno_sal1 { get; set; }
        public DateTime? f_rno_sal2 { get; set; }
        public string categoria { get; set; }
        public DateTime? f_asig_tec { get; set; }
        public DateTime? f_inspec { get; set; }
        public int NumeroIngresos
        {
            get
            {
                var ingresos = 1;
                if (this.f_r1_ing != null && this.f_r1_ing > f_ing_rec)
                {
                    ingresos = 2;
                }
                if (this.f_r2_ing != null && this.f_r2_ing > f_ing_rec)
                {
                    ingresos = 3;
                }
                return ingresos;
            }
        }
        public DateTime? FechaIngreso
        {
            get
            {
                var fecha = this.f_ing_rec;
                if (NumeroIngresos == 1)
                {
                    fecha = this.f_ing_rec;
                }
                if (NumeroIngresos == 2)
                {
                    fecha = this.f_r1_ing;
                }
                if (NumeroIngresos == 3)
                {
                    fecha = this.f_r2_ing;
                }
                return fecha;
            }
        }
        public int DiasIngreso1(List<DiasFestivos> dias)
        {
           
            diasFestivosList = dias;
            var misdias = 0;

            if (estado == "T")
            {
                misdias = DateTime.Now.Subtract(this.f_ing_rec.Value).Days;
                misdias = misdias - DiasAsumar(this.f_ing_rec.Value, DateTime.Now);
            }
            else
            {
                var fechaFinal = f_rno_rec;
                if (estado == "M")
                {
                    fechaFinal = f_rno_sal1;
                    if (fechaFinal < f_ing_rec) {
                        fechaFinal = retiromemo;
                    }
                }
                misdias = fechaFinal.Value.Subtract(this.f_ing_rec.Value).Days;
                misdias = misdias - DiasAsumar(this.f_ing_rec.Value, fechaFinal.Value);
            }

            return misdias;
        }
        public int DiasIngreso2(List<DiasFestivos> dias) {
            this.diasFestivosList = dias;
            var misdias = 0;
            if (this.NumeroIngresos > 1)
            {

                if (estado == "T")
                {
                    misdias = DateTime.Now.Subtract(this.f_r1_ing.Value).Days;
                    misdias = misdias - DiasAsumar(this.f_r1_ing.Value, DateTime.Now);
                }
                else
                {
                    var fechaFinal = f_rno_rec;
                    if (estado == "M") fechaFinal = f_rno_sal2;
                    misdias = fechaFinal.Value.Subtract(this.f_r1_ing.Value).Days;
                    misdias = misdias - DiasAsumar(this.f_r1_ing.Value, fechaFinal.Value);
                }

            }
            else {
                misdias = 0;
            }
            return misdias;
        }
        public int DiasIngreso3(List<DiasFestivos> dias) {
            this.diasFestivosList = dias;
            var misdias = 0;
            if (this.NumeroIngresos > 2)
            {
                if (estado == "T")
                {
                    misdias = DateTime.Now.Subtract(this.f_r2_ing.Value).Days;
                    misdias = misdias - DiasAsumar(this.f_r2_ing.Value, DateTime.Now);
                }
                else
                {
                    var fecha = this.f_r2_ing;
                    misdias = this.f_rno_rec.Value.Subtract(this.f_r2_ing.Value).Days;
                    misdias = misdias - DiasAsumar(this.f_r2_ing.Value, this.f_rno_rec.Value);
                }
            }
            else {
                misdias = 0;
            }
            return misdias;
        }
        List<DiasFestivos> diasFestivosList;
        public int DiasAsumar(DateTime fechaI, DateTime fechaF)
        {
            int dias = 0;

            for (DateTime fecha = fechaI; fecha <= fechaF; fecha = fecha.AddDays(1))
            {
                var arreglodias = this.diasFestivosList.Where(p => p.fecha == fecha).FirstOrDefault();
                if (arreglodias != null)
                {
                    dias++;
                }
                else if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                {
                    dias++;
                }
            }

            return dias;
        }
        public String uso_esp { get; set; }

        


    }


    public class TiemposMaximos {
        public decimal codigo { get; set; }
        public string tramite { get; set; }
        public string categoria { get; set; }
        public decimal t_max { get; set; }
        public decimal d_max { get; set; }
        public string descrip { get; set; }
        public string tipo { get; set; }
        
    }
}