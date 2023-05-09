package com.example.fjempleada.modelo

import android.app.AlertDialog
import android.content.DialogInterface
import android.content.Intent
import android.location.LocationManager
import android.provider.Settings
import com.example.fjempleada.R
import com.example.fjempleada.activitys.ui.Publicacion
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.model.MapStyleOptions

class mapaModel(context: Publicacion) {
    private var context:Publicacion=context

    internal fun gpsNotEnabled(manager: LocationManager){
        val builder = AlertDialog.Builder(context.context)
        builder.setMessage("Su gps no esta encendido, es necesario activarlo para el correcto funcionamiento de la aplicación")
        builder.setCancelable(false).setPositiveButton("SI") { _: DialogInterface, _: Int ->
            context.startActivity(Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS))
        }.create()
        builder.show()
    }
    internal fun estiloMapa(googleMap: GoogleMap?) {
        try {
            val succes:Boolean = googleMap!!.setMapStyle(MapStyleOptions.loadRawResourceStyle(context.context, R.raw.s))
            if(!succes){
            }
        } catch (e: Exception) {

        }
    }
}