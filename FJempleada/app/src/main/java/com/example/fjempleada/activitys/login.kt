package com.example.fjempleada.activitys

import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import android.os.Build
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.Toast
import androidx.annotation.RequiresApi
import com.example.fjempleada.R
import com.example.fjempleada.interfaces.ApiService
import com.example.fjempleada.modelo.activityModel
import com.example.fjempleada.modelo.startApi
import com.example.fjempleada.modelo.usuarioModel
import com.example.fjempleada.modelo.validationModel
import com.google.gson.Gson
import com.google.gson.JsonArray
import kotlinx.android.synthetic.main.activity_login.*
import org.json.JSONArray
import retrofit2.Call
import retrofit2.Response

class login : AppCompatActivity(),View.OnClickListener {

    val validacion = validationModel()
    val start = startApi()
    val usuario = usuarioModel()
    private lateinit var service: ApiService
    private val actividades = activityModel(this)

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)
        validacion.hideProgress(bar_login)
        validacion.context = this
        validacion.preferencias_user = getSharedPreferences(validacion.PREF_USUARIO, Context.MODE_PRIVATE)
        service = start.initApi()
        btnLogin.setOnClickListener (this)
        tvRegistrate.setOnClickListener(this)
    }
    override fun onClick(v: View?) {
        if(btnLogin==v){
            validacion.showProgress(bar_login)
            validacion.hideButton(btnLogin)
            if (!validateForm()) {
                validacion.showButton(btnLogin)
                validacion.hideProgress(bar_login)
            } else {
                usuario.email = txtEmail.text.toString()
                usuario.pass = txtPassword.text.toString()
                Login(usuario, validacion.preferencias_user!!)
            }

        }
        if (tvRegistrate==v){
            actividades.registro()
        }
    }

    fun Login(usuarioModel: usuarioModel, preferencias: SharedPreferences) {
        service.login(usuarioModel.email, usuarioModel.pass)
            .enqueue(object : retrofit2.Callback<JsonArray> {
                @RequiresApi(Build.VERSION_CODES.KITKAT)
                override fun onResponse(call: Call<JsonArray>, response: Response<JsonArray>) {
                    val json = Gson().toJson(response?.body())
                    val share = preferencias.edit()
                    val array = JSONArray(json)
                    for (i in 0 until array.length()) {
                        val row = array.getJSONObject(i)
                        share.putString("id", row.getString("id"))
                        share.putInt("idperfil", 3)
                        share.putString("nombre", row.getString("nombre"))
                        share.putString("apellido", row.getString("apellido"))
                        share.putString("telefono", row.getString("telefono"))
                        share.putInt("idpersona", row.getString("idpersona").toInt())
                        share.putString("sexo", row.getString("sexo"))
                        share.putString("curp", row.getString("curp"))
                        share.putString("fechanacimiento", row.getString("fechanacimiento"))
                        share.putString("longi", row.getString("longi"))
                        share.putString("lat", row.getString("lat"))
                        share.putString("idinteres", row.getString("idinteres"))
                        share.putString("fotoperfil", row.getString("fotoperfil"))
                        share.putString("email", usuarioModel.email)
                        share.putString("pass", usuarioModel.pass)
                        share.commit()
                    }
                    val intento = Intent(applicationContext, permisos::class.java)
                    startActivity(intento)
                    finish()
                    validacion.showButton(btnLogin)
                }

                override fun onFailure(call: Call<JsonArray>, t: Throwable) {
                    Toast.makeText(
                        applicationContext,
                        t.toString(),
                        Toast.LENGTH_SHORT
                    ).show()
                    validacion.hideProgress(bar_login)
                    validacion.showButton(btnLogin)


                }
            })
    }

    fun Registrarse(v: View) {
        try {
            val intent = Intent(applicationContext, registro::class.java)
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP)
            startActivity(intent)
        } catch (e: Exception) {
            e.printStackTrace()
        }
    }

    private fun validateForm(): Boolean {
        if (!validacion.Validate(txtEmail, txtLayoutEmail, "email", R.string.valid_email)) {
            return false
        }
        if (!validacion.Validate(txtPassword, txtLayoutPassword, "pass", R.string.valid_pass)) {
            return false
        }
        return true
    }


}
