package com.example.fjempleada.modelo

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.widget.Toast
import com.example.fjempleada.activitys.*

class activityModel(context: Context) {
    private var context=context
    private var intent:Intent? = null
    private val validacion=validationModel()
    internal fun login(){
        intent= Intent(context, login::class.java)
        context.startActivity(intent)
    }
    internal fun registro(){
        intent= Intent(context, registro::class.java)
        context.startActivity(intent)
    }
    internal fun permiso(){
        intent= Intent(context, permisos::class.java)
        context.startActivity(intent)
    }
    internal fun menu(){
        intent= Intent(context, menu::class.java)
        context.startActivity(intent)
    }
    internal fun showMensajeToast(mensaje:String){
        Toast.makeText(context, mensaje, Toast.LENGTH_SHORT).show()
    }
    internal fun validarSesion() {
        validacion.preferencias_user =
            context.getSharedPreferences(validacion.PREF_USUARIO, Context.MODE_PRIVATE)
        if (validacion!!.preferencias_user!!.contains("id")) {
        } else {
            val editor = validacion!!.preferencias_user?.edit()
            editor?.clear()?.commit()
            login()
            (context as Activity).finish()
        }
    }

}