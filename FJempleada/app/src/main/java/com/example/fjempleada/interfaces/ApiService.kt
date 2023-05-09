package com.example.fjempleada.interfaces

import com.google.gson.JsonArray
import okhttp3.MultipartBody
import retrofit2.Call
import retrofit2.http.*

interface ApiService {
    @FormUrlEncoded
    @POST("Service.asmx/Login")
    fun login(@Field("email") email: String, @Field("pass") pass: String, @Field("perfil") perfil: Int = 3): Call<JsonArray>

    @FormUrlEncoded
    @POST("Service.asmx/AddUsuario")
    fun AddUsuario(
        @Field("email") email: String, @Field("pass") pass: String, @Field("idperfil") idperfil: Int = 3,
        @Field("nombre") nombre: String, @Field("apellido") apellido: String,
        @Field("telefono") telefono: String, @Field("sexo") sexo: String, @Field("curp") curp: String,
        @Field("fechanacimiento") fechanacimiento: String, @Field("fotoperfil") fotoperfil: String, @Field(
            "longi"
        ) longi: String, @Field("lat") lat: String,
        @Field("idinteres") idinteres: Int
    ): Call<String>

    @FormUrlEncoded
    @POST("Service.asmx/GeTPublicacion")
    fun GetPublicacion(@Field("id") id: String): Call<JsonArray>

    @FormUrlEncoded
    @POST("Service.asmx/GetPersona")
    fun GetPersona(@Field("id") id: Int): Call<JsonArray>

    @FormUrlEncoded
    @POST("Service.asmx/Addcalificacion")
    fun Addcalificacion(
        @Field("idusuario") idusuario: Int, @Field("puntuacion") puntuacion: Double, @Field(
            "comentario"
        ) comentario: String
    ): Call<String>

    @FormUrlEncoded
    @POST("Service.asmx/Addinsidencia")
    fun Addinsidencia(
        @Field("mensaje") mensaje: String, @Field("idusuario") idusuario: Int,
        @Field("idusreportado") idusreportado: Int, @Field("estatus") estatus: Int,
        @Field("tipo") tipo: Int, @Field("fecha") fecha: String
    ): Call<String>

    @POST("Service.asmx/GetCategorias")
    fun getCategorias(): Call<JsonArray>


    @Multipart
    @POST("Service.asmx/UploadFile")
    fun loadImage(
        @Part file: MultipartBody.Part
    ): Call<String>

    @FormUrlEncoded
    @POST("Service.asmx/uploadfotoperfil")
    fun Updatefotoperfil(
        @Field("img") img: String, @Field("id") id: Int
    ): Call<String>
    @FormUrlEncoded
    @POST("Service.asmx/validarEmail")
    fun validarEmail(
        @Field("email") email: String
    ): Call<String>


}