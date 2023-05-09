package com.example.fjempleada.adapter

import android.content.Context
import android.content.Intent
import android.graphics.BitmapFactory
import android.graphics.Color
import android.util.Base64
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import androidx.localbroadcastmanager.content.LocalBroadcastManager
import androidx.recyclerview.widget.RecyclerView
import com.example.fjempleada.R
import com.example.fjempleada.modelo.interesModel
import kotlinx.android.synthetic.main.item_interes.view.*
import kotlinx.android.synthetic.main.nav_header_menu.view.*

class adpInteres(context: Context, lista: List<interesModel>) :
    RecyclerView.Adapter<adpInteres.MyViewHolder>() {
    val context: Context? = context
    val lista: List<interesModel> = lista
    var row_index: Int = -1

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): MyViewHolder {
        val view: View
        val layout: LayoutInflater = LayoutInflater.from(context)
        view = layout.inflate(R.layout.item_interes, parent, false)

        return MyViewHolder(view)
    }

    override fun getItemCount(): Int {
        return lista.size
    }

    override fun onBindViewHolder(holder: MyViewHolder, position: Int) {
        holder.txtNombreInteres.text = lista[position].nombre
        val d = Base64.decode(lista[position].icono, Base64.DEFAULT)
        val bm = BitmapFactory.decodeByteArray(d, 0, d.size)
        holder.img.setImageBitmap(bm)
        holder.card_action.setOnClickListener {
            val intent = Intent("mensaje-id")
            intent.putExtra("id", lista.get(position).id)
            LocalBroadcastManager.getInstance(this!!.context!!).sendBroadcast(intent)
            row_index = position
            notifyDataSetChanged()
        }
        if (row_index == position) {
            holder.card_action.setBackgroundColor(Color.parseColor("#FF4081"))
            holder.txtNombreInteres.setTextColor(Color.parseColor("#FFFFFF"))
        } else {
            holder.card_action.setBackgroundColor(Color.parseColor("#FFFFFF"))
            holder.txtNombreInteres.setTextColor(Color.parseColor("#FF4081"))
        }

    }

    class MyViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val txtNombreInteres = itemView.txtNombreInteres
        val card_action = itemView.cardView
        val img: ImageView = itemView.imageView
    }
}