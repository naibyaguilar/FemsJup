package com.example.fjempleada.activitys.ui


import android.annotation.SuppressLint
import android.content.Context
import android.location.Criteria
import android.location.Location
import android.location.LocationManager
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup

import com.example.fjempleada.R
import com.example.fjempleada.modelo.mapaModel
import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.OnMapReadyCallback
import com.google.android.gms.maps.model.LatLng
import kotlinx.android.synthetic.main.fragment_publicacion.view.*

/**
 * A simple [Fragment] subclass.
 */
class Publicacion : Fragment(), OnMapReadyCallback {

    private var root:View? = null
    private var locationManager:LocationManager?=null
    var criteria:Criteria? =null
    var provider:String? = null
    var mylocation: Location? = null
    private var mapa=mapaModel(this)
    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        // Inflate the layout for this fragment
        root= inflater.inflate(R.layout.fragment_publicacion, container, false)
        if (root!!.mapubicacion != null) {
            root!!.mapubicacion.onCreate(null)
            root!!.mapubicacion.onResume()
            root!!.mapubicacion.getMapAsync(this)
        }



        return root
    }

    @SuppressLint("MissingPermission")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        locationManager = context?.getSystemService(Context.LOCATION_SERVICE) as LocationManager
        criteria = Criteria()
        provider = locationManager?.getBestProvider(criteria, false)
        mylocation = locationManager?.getLastKnownLocation(provider)
        validarGPS()
    }
    private fun validarGPS(){
        if (!locationManager!!.isProviderEnabled(LocationManager.GPS_PROVIDER)){
            mapa.gpsNotEnabled(locationManager!!)
        }
    }

    override fun onMapReady(google: GoogleMap?) {
        try {
            mapa.estiloMapa(google)
            google!!.isMyLocationEnabled=true
            google!!.uiSettings.isMyLocationButtonEnabled=true
            val yo = LatLng(mylocation!!.latitude, mylocation!!.longitude)
            google.moveCamera(CameraUpdateFactory.newLatLngZoom(yo,14F))
        }catch (e:Exception){}

     }

}
