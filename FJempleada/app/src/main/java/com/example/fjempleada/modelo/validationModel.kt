package com.example.fjempleada.modelo

import android.app.Activity
import android.content.SharedPreferences
import android.util.Patterns
import android.view.View
import android.widget.Button
import android.widget.EditText
import android.widget.ProgressBar
import android.widget.Toast
import com.example.fjempleada.R
import com.google.android.material.textfield.TextInputLayout
import java.util.regex.Pattern

class validationModel {
    var context: Activity? = null
    var PASSWORD_PATTERN: Pattern =
        Pattern.compile("^(?=.{6,15}\$).*")//expresion regular para min y max de contraseña
    var preferencias_user: SharedPreferences?=null
    var preferencias_permiso: SharedPreferences?=null
    var PREF_USUARIO="usuario"
    var PREF_PERMISO="permiso"
    val INTERVALO=2000 //2 Segundos para salir
    var tiempoPrimerClick:Long=0

    fun Validate(editText: EditText, textInputLayout: TextInputLayout, inputType: String, errorMensaje: Int): Boolean{
        if (editText.text.toString().trim().isEmpty()){
            textInputLayout.error= context?.getString(R.string.valid_campo)
            return false
        }else{
            if(inputType=="email"){
                if (!Patterns.EMAIL_ADDRESS.matcher(editText.text.toString()).matches()) {
                    textInputLayout.error = context?.getString(errorMensaje)
                    return false
                }
            }
            if (inputType == "pass") {
                if (!PASSWORD_PATTERN.matcher(editText.text.toString()).matches()) {
                    textInputLayout.error = context?.getString(errorMensaje)
                    return false
                }
            }
            textInputLayout.error = null
        }
        return true
    }

    fun ValidateRegistro(editText: EditText, inputType: String, errorMensaje: Int): Boolean {
        if (editText.text.toString().trim().isEmpty()) {
            editText.error = context?.getString(R.string.valid_campo)
            return false
        } else {
            if (inputType == "email") {
                if (!Patterns.EMAIL_ADDRESS.matcher(editText.text.toString()).matches()) {
                    editText.error = context?.getString(errorMensaje)
                    return false
                }
            }
            if (inputType == "pass") {
                if (!PASSWORD_PATTERN.matcher(editText.text.toString()).matches()) {
                    editText.error = context?.getString(errorMensaje)
                    return false
                }
            }
            editText.error = null
        }
        return true
    }

    fun showButton(btn: Button) {
        btn.visibility = View.VISIBLE
    }

    fun hideButton(btn: Button) {
        btn.visibility = View.INVISIBLE
    }

    fun hideProgress(progress: ProgressBar) {
        progress.visibility = View.INVISIBLE
    }

    fun showProgress(progress: ProgressBar) {
        progress.visibility = View.VISIBLE
    }

    fun mensaje(mensaje: Int) {
        Toast.makeText(
            context?.applicationContext,
            context?.getString(mensaje),
            Toast.LENGTH_SHORT
        ).show()
    }



}