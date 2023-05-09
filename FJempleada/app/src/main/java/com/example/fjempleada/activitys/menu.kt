package com.example.fjempleada.activitys

import android.content.Context
import android.content.Intent
import android.os.Bundle
import com.google.android.material.floatingactionbutton.FloatingActionButton
import com.google.android.material.snackbar.Snackbar
import androidx.navigation.findNavController
import androidx.navigation.ui.AppBarConfiguration
import androidx.navigation.ui.navigateUp
import androidx.navigation.ui.setupActionBarWithNavController
import androidx.navigation.ui.setupWithNavController
import androidx.drawerlayout.widget.DrawerLayout
import com.google.android.material.navigation.NavigationView
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import android.view.Menu
import android.view.MenuItem
import androidx.core.view.GravityCompat
import androidx.fragment.app.Fragment
import com.example.fjempleada.R
import com.example.fjempleada.activitys.ui.Historial
import com.example.fjempleada.activitys.ui.Home
import com.example.fjempleada.activitys.ui.Perfil
import com.example.fjempleada.activitys.ui.Publicacion
import com.example.fjempleada.modelo.activityModel
import com.example.fjempleada.modelo.validationModel
import com.squareup.picasso.Picasso
import kotlinx.android.synthetic.main.nav_header_menu.view.*

class menu : AppCompatActivity(), NavigationView.OnNavigationItemSelectedListener {
      private lateinit var appBarConfiguration: AppBarConfiguration
    private var validacion = validationModel()
    private  val actividades = activityModel(this)
    private var drawerLayout: DrawerLayout? = null


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_menu)
        val toolbar: Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(toolbar)
        LoadFragment(Home())
        validacion.preferencias_user = getSharedPreferences(validacion.PREF_USUARIO, Context.MODE_PRIVATE)
        drawerLayout = findViewById(R.id.drawer_layout)
        val navView: NavigationView = findViewById(R.id.nav_view)
        navView!!.setNavigationItemSelectedListener(this)
        val navController = findNavController(R.id.nav_host_fragment)

        val view = navView.getHeaderView(0)
        Picasso.with(this).load(validacion!!.preferencias_user?.getString("fotoperfil", "").toString()).into(view.img_nav_perfil)
        view.tv_nav_nombre.text = validacion!!.preferencias_user?.getString("nombre", "").toString()


        appBarConfiguration = AppBarConfiguration(
            setOf(
                R.id.nav_home, R.id.nav_perfil, R.id.nav_publicacion,
                R.id.nav_historial, R.id.nav_CV, R.id.nav_sesion
            ), drawerLayout
        )
        setupActionBarWithNavController(navController, appBarConfiguration)
    }

    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        // Inflate the menu; this adds items to the action bar if it is present.
        menuInflater.inflate(R.menu.menu, menu)
        return true
    }

    override fun onSupportNavigateUp(): Boolean {
        val navController = findNavController(R.id.nav_host_fragment)
        return navController.navigateUp(appBarConfiguration) || super.onSupportNavigateUp()
    }
    override fun onNavigationItemSelected(item: MenuItem): Boolean {
        when (item.itemId){
            R.id.nav_home->{ LoadFragment(Home())}
            R.id.nav_perfil->{ LoadFragment(Perfil())}
            R.id.nav_publicacion->{ LoadFragment(Publicacion())}
            R.id.nav_historial->{ LoadFragment(Historial())}
            R.id.nav_sesion->{
            val share=validacion.preferencias_user!!.edit().clear()
                share.commit()
                actividades.login()
                finish()
            }
        }

        drawerLayout!!.closeDrawer(GravityCompat.START)
        return true
    }
    private fun LoadFragment(fragment: Fragment){
        val fragmentManager = supportFragmentManager
        val fragmentTransaction =  fragmentManager.beginTransaction()
        fragmentTransaction.replace(R.id.nav_host_fragment, fragment).addToBackStack(null)
        fragmentTransaction.commit()
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        for (fragment in supportFragmentManager.fragments) {
            fragment.onActivityResult(requestCode, resultCode, data)
        }
    }
}
