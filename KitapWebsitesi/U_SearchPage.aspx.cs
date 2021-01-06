﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace KitapWebsitesi
{
    public partial class U_SearchPage : System.Web.UI.Page
    {
        sqlConnectionClass conn = new sqlConnectionClass();
        string search = "";
        string addBasket = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_sum.Visible = false;
            search = Request.QueryString["search"];
            SqlCommand cd = new SqlCommand("select *from Tbl_Kitaplar where KitapAd like @p1 or KitapYazar like @p1 or KitapOrijinalAd like @p1 or KitapCevirmen like @p1 or KitapYayinevi like @p1", conn.connection());
            cd.Parameters.AddWithValue("@p1", "%" + search + "%");
            SqlDataReader dr = cd.ExecuteReader();
            if (dr.HasRows)
            {
                dl_search.DataSource = dr;
                dl_search.DataBind();
            }
            else
            {
                lbl_sum.Visible = true;
            }
            conn.connection().Close();

        }

        protected void btn_sepet1_Click(object sender, EventArgs e)
        {
            if (Session["id"] != null)
            {
                string current = "";
                double bookprice = 0;
                string bookName = "";
                SqlCommand cdquery = new SqlCommand("select *from Tbl_Sepet where KitapId=@p1 and KullaniciId=@p2 and SiparisNo is null", conn.connection());
                cdquery.Parameters.AddWithValue("@p1", addBasket);
                cdquery.Parameters.AddWithValue("@p2", Session["id"]);
                SqlDataReader drquery = cdquery.ExecuteReader();
                while (drquery.Read())
                    current = drquery[0].ToString();
                conn.connection().Close();

                SqlCommand cdprice = new SqlCommand("select *from Tbl_Kitaplar where KitapId=@p1", conn.connection());
                cdprice.Parameters.AddWithValue("@p1", addBasket);
                SqlDataReader drprice = cdprice.ExecuteReader();
                while (drprice.Read())
                {
                    bookprice = Convert.ToDouble(drprice[4]);
                    bookName = drprice[1].ToString();
                }
                conn.connection().Close();

                if (current == "")
                {
                    SqlCommand cd = new SqlCommand("insert into Tbl_Sepet(KitapId,SepetFiyat,KullaniciId,KitapFiyat,KitapAd) values (@p1,@p2,@p3,@p4,@p5)", conn.connection());
                    cd.Parameters.AddWithValue("@p1", addBasket);
                    cd.Parameters.AddWithValue("@p2", bookprice);
                    cd.Parameters.AddWithValue("@p3", Session["id"]);
                    cd.Parameters.AddWithValue("@p4", bookprice);
                    cd.Parameters.AddWithValue("@p5", bookName);
                    cd.ExecuteNonQuery();
                    conn.connection().Close();
                }
                else
                {
                    SqlCommand cd = new SqlCommand("update Tbl_Sepet set SepetMiktar+=1 where SepetId=@p1 and SiparisNo is null", conn.connection());
                    cd.Parameters.AddWithValue("@p1", current);
                    cd.ExecuteNonQuery();
                    conn.connection().Close();

                    SqlCommand cdtf = new SqlCommand("update Tbl_Sepet set SepetFiyat= KitapFiyat*SepetMiktar where SepetId=@p1 and SiparisNo is null", conn.connection());
                    cdtf.Parameters.AddWithValue("@p1", current);
                    cdtf.ExecuteNonQuery();
                    conn.connection().Close();
                }

            }
            else
            {
                Response.Redirect("U_Login.aspx");
            }
        }
    }
}