package com.example.fjempleada.activitys

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.example.fjempleada.R
import com.example.fjempleada.modelo.permisoModel
import kotlinx.android.synthetic.main.activity_permisos.*

class permisos : AppCompatActivity() {
    val permisos = permisoModel(this)
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_permisos)
        permisos.validarPermisoAcvityPermiso()
        btn_allow_permisos.setOnClickListener {
            permisos.permisosValidacion()
        }
    }
}
