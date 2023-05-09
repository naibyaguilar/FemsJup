package com.example.fjempleada.modelo

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.net.Uri
import android.os.Build
import android.os.Environment
import android.provider.MediaStore
import androidx.core.content.FileProvider
import okhttp3.MediaType
import okhttp3.MultipartBody
import okhttp3.RequestBody
import java.io.ByteArrayOutputStream
import java.io.File
import kotlin.math.ceil
import kotlin.math.max

class fotoModel(context: Context) {
    private var ruta:String ="proyecto5cuatri-1/imagenes/Front"
    internal var path:String =""
    internal var nombre:String =""
    internal var uri: Uri?=null
    internal var context = context

    internal fun TakePicture(){
        val file = File(Environment.getExternalStorageDirectory(), ruta)
        var isCreate =file.exists()
        if (!isCreate){
            isCreate = file.mkdirs()
        }
        if(isCreate){
            nombre = (System.currentTimeMillis() / 1000).toString() + ".jpg"
        }
        path = Environment.getExternalStorageDirectory().toString() + File.separator + ruta + File.separator + nombre
        val imagen = File(path)
        var intent = Intent(MediaStore.ACTION_IMAGE_CAPTURE)
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N){
            val autoridad:String =context.packageName +".fileprovider"
            uri = FileProvider.getUriForFile(context, autoridad, imagen)
            intent.putExtra(MediaStore.EXTRA_OUTPUT, uri)
        }else{
            intent.putExtra(MediaStore.EXTRA_OUTPUT, Uri.fromFile(imagen))
            uri = Uri.fromFile(imagen)
        }
        (context as Activity).startActivityForResult(intent, 29)
    }
    internal fun reducirResolucion(uri: String, maxAncho:Int, maxAlto:Int): Bitmap? {
        val opciones = BitmapFactory.Options()
        opciones.inJustDecodeBounds = true
        BitmapFactory.decodeStream(context.contentResolver.openInputStream(Uri.parse(uri)),null, opciones)
        opciones.inSampleSize = max(ceil((opciones.outWidth/maxAncho).toDouble()), ceil((opciones.outHeight / maxAlto).toDouble())).toInt()
        opciones.inJustDecodeBounds = false
        return BitmapFactory.decodeStream(context.contentResolver.openInputStream(Uri.parse(uri)), null, opciones)
    }
    internal fun reducirPeso(imagen: Bitmap?): Uri?{
        val bytes = ByteArrayOutputStream()
        imagen?.compress(Bitmap.CompressFormat.JPEG, 100, bytes)
        val ruta = MediaStore.Images.Media.insertImage(context.contentResolver, imagen,  (System.currentTimeMillis() / 1000).toString(), null)
        return Uri.parse(ruta)
    }
    internal fun getImagenURL(uri: Uri?):String{
        val cursor = context.contentResolver.query(uri!!, null, null, null, null)
        cursor?.moveToFirst()
        val id =cursor?.getColumnIndex(MediaStore.Images.ImageColumns.DATA)
        return cursor!!.getString(id!!)
    }
    internal fun multiPart(file: File): MultipartBody.Part?{
        return MultipartBody.Part.createFormData(
            "file",
            file.name,
            RequestBody.create(MediaType.parse("image/*"), file)
        )
    }
    internal fun abrirGaleria() {
        val gallery = Intent(Intent.ACTION_PICK, MediaStore.Images.Media.INTERNAL_CONTENT_URI)
        (context as Activity).startActivityForResult(gallery, 10)
    }
}