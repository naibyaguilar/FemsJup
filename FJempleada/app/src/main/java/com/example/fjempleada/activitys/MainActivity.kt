package com.example.fjempleada.activitys

import android.content.Context
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.animation.AnimationUtils
import com.example.fjempleada.R
import com.example.fjempleada.modelo.activityModel
import com.example.fjempleada.modelo.permisoModel
import com.example.fjempleada.modelo.validationModel
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : AppCompatActivity() {
    private val validacion = validationModel()
    private val permiso = permisoModel(this)
    private val actividades = activityModel(this)


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        val animation = AnimationUtils.loadAnimation(this, R.anim.zoom)
        imgSplash.startAnimation(animation)
        var intent: Intent?=null
        validacion.preferencias_user=getSharedPreferences(validacion.PREF_USUARIO, Context.MODE_PRIVATE)
        validacion.preferencias_permiso = getSharedPreferences(validacion.PREF_PERMISO, Context.MODE_PRIVATE)
        val timer = object :Thread(){
            override fun run() {
                try {
                    sleep(3000)
                } catch (e: InterruptedException) {
                    e.printStackTrace()
                } finally {
                    if(validacion.preferencias_user!!.contains("id")) {
                        if (validacion.preferencias_permiso!!.contains("crear")){
                            permiso.validarPermisoSplash()
                        }else{
                            actividades.permiso()
                            finish()
                        }
                    }else{
                        actividades.login()
                        finish()
                    }
                }
            }
        }
        timer.start()
    }
}
