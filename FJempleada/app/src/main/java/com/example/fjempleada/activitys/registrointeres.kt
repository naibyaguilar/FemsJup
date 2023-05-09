package com.example.fjempleada.activitys

import android.content.*
import android.os.Build
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Toast
import androidx.annotation.RequiresApi
import androidx.localbroadcastmanager.content.LocalBroadcastManager
import androidx.recyclerview.widget.GridLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.fjempleada.R
import com.example.fjempleada.adapter.adpInteres
import com.example.fjempleada.interfaces.ApiService
import com.example.fjempleada.modelo.interesModel
import com.example.fjempleada.modelo.startApi
import com.example.fjempleada.modelo.usuarioModel
import com.example.fjempleada.modelo.validationModel
import com.google.gson.Gson
import com.google.gson.JsonArray
import kotlinx.android.synthetic.main.activity_login.*
import kotlinx.android.synthetic.main.activity_registrointeres.*
import org.json.JSONArray
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class registrointeres : AppCompatActivity() {
    val validacion = validationModel()
    val usuario = usuarioModel()
    val start = startApi()
    val interes = interesModel()
    private lateinit var service: ApiService
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_registrointeres)
        validacion.hideProgress(bar_registroInte)
        validacion.context = this
        validacion.preferencias_user = getSharedPreferences(validacion.PREF_USUARIO, Context.MODE_PRIVATE)
        service = start.initApi()
        getCategorias()
        var intent = getIntent().extras
        if (intent != null) {
            usuario.nombre = intent.getString("nombre", "")
            usuario.apellido = intent.getString("apellido", "")
            usuario.email = intent.getString("email", "")
            usuario.pass = intent.getString("pass", "")
            usuario.sexo = intent.getString("sexo", "")
            usuario.telefono = intent.getString("telefono", "")
            usuario.fotoperfil = "https://image.flaticon.com/icons/svg/706/706814.svg"
            usuario.idperfil = 3
        }
        LocalBroadcastManager.getInstance(this)
            .registerReceiver(mensajeRecibido, IntentFilter("mensaje-id"))

        btnFinalizarRegistro.setOnClickListener {
            validacion.showProgress(bar_registroInte)
            validacion.hideButton(btnFinalizarRegistro)
            if (usuario.idinteres != 0) {
                AddUsuario(usuario,validacion.preferencias_user!!)
            } else {
                Toast.makeText(
                    applicationContext,
                    "Selececione alguna categoria de preferencia.",
                    Toast.LENGTH_SHORT
                ).show()
                validacion.showButton(btnFinalizarRegistro)
                validacion.hideProgress(bar_registroInte)
            }
        }
    }
    fun AddUsuario(usuario: usuarioModel, preferencias: SharedPreferences) {
        service.AddUsuario(
            usuario.email,
            usuario.pass,
            usuario.idperfil,
            usuario.nombre,
            usuario.apellido,
            usuario.telefono,
            usuario.sexo,
            "",
            usuario.fechanacimiento,
            usuario.fotoperfil,
            "",
            "",
            usuario.idinteres
        ).enqueue(object : Callback<String> {
            @RequiresApi(Build.VERSION_CODES.KITKAT)
            override fun onFailure(call: Call<String>, t: Throwable) {
                Toast.makeText(
                    applicationContext,
                    "EL suario no exite o no es el perfil correcto.",
                    Toast.LENGTH_SHORT
                ).show()
                validacion.hideProgress(bar_login)
                validacion.showButton(btnLogin)
            }

            override fun onResponse(call: Call<String>, response: Response<String>) {
                val share = preferencias.edit()
                share.putString("id", response.body())
                share.putInt("idperfil", 3)
                share.putString("nombre", usuario.nombre)
                share.putString("apellido", usuario.apellido)
                share.putString("telefono", usuario.telefono)
                share.putString("sexo", usuario.sexo)
                share.putString("curp", usuario.curp)
                share.putString("fechanacimiento", usuario.fechanacimiento)
                share.putString("longi", usuario.longi)
                share.putString("lat", usuario.lat)
                share.putString("idinteres", usuario.idinteres.toString())
                share.putString("fotoperfil", usuario.fotoperfil)
                share.putString("email", usuario.email)
                share.putString("pass", usuario.pass)
                share.commit()
                val intento = Intent(applicationContext, permisos::class.java)
                startActivity(intento)
                finish()
            }
        })
    }

    var mensajeRecibido: BroadcastReceiver = object : BroadcastReceiver() {
        override fun onReceive(context: Context, intent: Intent) {
            usuario.idinteres = intent.getStringExtra("id").toInt()
        }
    }

    fun getCategorias() {
        service.getCategorias().enqueue(object : Callback<JsonArray> {
            override fun onResponse(call: Call<JsonArray>, response: Response<JsonArray>) {
                var rcv: RecyclerView? = null
                var adp: adpInteres? = null
                val lista = ArrayList<interesModel>()
                val json = Gson().toJson(response?.body())
                val array = JSONArray(json)
                for (i in 0 until array.length()) {
                    val row = array.getJSONObject(i)
                    interes.id = row.getString("id").toString()
                    interes.nombre = row.getString("nombre").toString()
                    interes.icono = row.getString("icono")
                    lista.add(interesModel(interes.id, interes.nombre, interes.icono))
                    rcv = rcvInteres
                    adp = adpInteres(applicationContext, lista)
                }
                rcv?.layoutManager = GridLayoutManager(applicationContext, 2)
                rcv?.adapter = adp
            }
            override fun onFailure(call: Call<JsonArray>, t: Throwable) {

            }


        })
    }
}
