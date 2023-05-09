package com.example.fjempleada.activitys

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.RadioButton
import android.widget.Toast
import com.example.fjempleada.R
import com.example.fjempleada.interfaces.ApiService
import com.example.fjempleada.modelo.startApi
import com.example.fjempleada.modelo.usuarioModel
import com.example.fjempleada.modelo.validationModel
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_registro.*
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class registro : AppCompatActivity(), View.OnClickListener  {
    val usuario = usuarioModel()
    private val validacion = validationModel()
    private val start = startApi()
    private lateinit var service: ApiService
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_registro)
        service = start.initApi()
        validacion.context = this
        validacion.hideProgress(bar_registro)
        btnRegistrarse.setOnClickListener(this)
    }
    override fun onClick(v: View?) {
        if (btnRegistrarse == v){
            validacion.showProgress(bar_registro)
            validacion.hideButton(btnRegistrarse)
            if (!validateForm()) {
                validacion.showButton(btnRegistrarse)
                validacion.hideProgress(bar_registro)
            } else {
                validarEmail(txtCorreo.text.toString())
            }
        }
    }
    private fun validateForm(): Boolean {
        if (!validacion.ValidateRegistro(txtNombre, "", R.string.valid_campo)) {
            return false
        }
        if (!validacion.ValidateRegistro(txtApellido, "", R.string.valid_campo)) {
            return false
        }
        if (!validacion.ValidateRegistro(txtCorreo, "email", R.string.valid_email)) {
            return false
        }
        if (!validacion.ValidateRegistro(txtpass, "pass", R.string.valid_pass)) {
            return false
        }
        if (!validacion.ValidateRegistro(txtConfirmar, "pass", R.string.valid_pass)) {
            return false
        }
        return true
    }
    private fun validarEmail(email:String){
        service.validarEmail(email)
            .enqueue(object : Callback<String> {
                override fun onResponse(call: Call<String>, response: Response<String>) {
                    val json = Gson().toJson(response?.body())
                    val nuevo = json.replace("\"", "")
                    if (nuevo!="1"){
                        if (txtpass.text.toString() == txtConfirmar.text.toString()) {
                            if (rgp_sexo.checkedRadioButtonId != null) {
                                val radio: RadioButton = findViewById(rgp_sexo.checkedRadioButtonId)
                                val intento = Intent(applicationContext, registrointeres::class.java)
                                intento.putExtra("nombre", txtNombre.text.toString())
                                intento.putExtra("apellido", txtApellido.text.toString())
                                intento.putExtra("email", txtCorreo.text.toString())
                                intento.putExtra("pass", txtpass.text.toString())
                                intento.putExtra("sexo", radio.text.toString())
                                intento.putExtra("telefono", txttelefono.text.toString())
                                startActivity(intento)
                                validacion.showButton(btnRegistrarse)
                                finish()
                            } else {
                                validacion.mensaje(R.string.valid_rbt)
                                validacion.showButton(btnRegistrarse)
                                validacion.hideProgress(bar_registro)
                            }

                        } else {
                            validacion.mensaje(R.string.valid_passmacht)
                            validacion.showButton(btnRegistrarse)
                            validacion.hideProgress(bar_registro)
                        }
                    }else{
                        txtCorreo.error ="Este correo ya ha sido registrado"
                        validacion.showButton(btnRegistrarse)
                        validacion.hideProgress(bar_registro)
                    }

                }
                override fun onFailure(call: Call<String>, t: Throwable) {
                    Toast.makeText(applicationContext, t.toString(), Toast.LENGTH_LONG).show()
                }
            })
    }


}
