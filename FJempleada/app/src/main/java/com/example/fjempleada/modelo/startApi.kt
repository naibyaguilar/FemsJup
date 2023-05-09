package com.example.fjempleada.modelo

import com.example.fjempleada.interfaces.ApiService
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class startApi {
    private lateinit var service: ApiService
    internal fun initApi(): ApiService {
        val retrofit: Retrofit = Retrofit.Builder()
            //.baseUrl("http://10.0.2.2/webservices/")
            .baseUrl("http://alexander14-001-site1.dtempurl.com/")
            .addConverterFactory(GsonConverterFactory.create())
            .build()
        service = retrofit.create<ApiService>(ApiService::class.java)
        return service
    }
}