# 📦 Sistema de Control y Gestión de Stock

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=flat&logo=dotnet)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-SQLite-68217A?style=flat&logo=.net)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat&logo=bootstrap)
![License](https://img.shields.io/badge/License-MIT-blue.svg)

Sistema web para el control de inventario, administración de categorías, gestión de usuarios y trazabilidad de movimientos de mercadería (Ingresos y Egresos) mediante remitos multi-artículo imprimibles.

---

## 📋 Tabla de Contenidos
- [Características Principales](#-características-principales)
- [Arquitectura y Stack Tecnológico](#-arquitectura-y-stack-tecnológico)
- [Modelo de Base de Datos](#-modelo-de-base-de-datos)
- [Requisitos Previos](#-requisitos-previos)
- [Instrucciones de Instalación y Ejecución](#-instrucciones-de-instalación-y-ejecución)
- [Estructura del Proyecto](#-estructura-del-proyecto)

---

## 🚀 Características Principales

1. **Gestión de Inventario (CRUD Completo):**
   - Administración de Productos y Categorías.
   - Control dinámico de stock disponible en tiempo real.
   - Búsqueda y filtrado rápido de mercadería.

2. **Control de Movimientos de Stock:**
   - Registro automatizado de **Ingresos** (Entrada) y **Egresos** (Salida/Retiro).
   - Ajuste y validación atómica de inventario según el tipo de operación.
   - Control defensivo para evitar salidas sin stock suficiente.

3. **Módulo de Remitos Multi-Artículo (Master-Detail):**
   - Carga dinámica en cliente (JavaScript) de múltiples productos por comprobante.
   - Transaccionalidad completa (`BeginTransactionAsync`): confirmación atómica o rollback en caso de fallos.
   - Vista optimizada de remito físico con firma de conformidad para impresión directa (`@media print`).

4. **Autenticación y Seguridad:**
   - Acceso seguro mediante autenticación basada en Cookies y Sesiones (`HttpContext.Session`).
   - Cifrado de contraseñas mediante **BCrypt**.
   - Control de acceso por roles (Administrador / Usuario).
   - Protección contra ataques CSRF mediante `[ValidateAntiForgeryToken]`.

---

## 🛠️ Arquitectura y Stack Tecnológico

- **Lenguaje:** C# (.NET 8 / .NET Framework equivalente)
- **Framework Web:** ASP.NET Core MVC (Model-View-Controller)
- **ORM / Persistencia:** Entity Framework Core (Enfoque Code-First / Database-First Mapped)
- **Motor de Base de Datos:** SQLite
- **Interfaz Gráfica (Frontend):** Razor Views (.cshtml), Bootstrap 5, JavaScript Vanilla
- **Manejo de Sesión:** Cookies de Autenticación y ASP.NET Core Session Middleware

---

## 🗄️ Modelo de Base de Datos

El sistema implementa la relación relacional entre las entidades principales:

```text
  ┌──────────────┐       ┌──────────────┐
  │  CATEGORIAS  │ 1───N │  PRODUCTOS   │
  └──────────────┘       └──────┬───────┘
                                │ 1
                                │
                                │ N
  ┌──────────────┐       ┌──────┴───────┐
  │   REMITOS    │ 1───N │DETALLES_REMITO│
  └──────┬───────┘       └──────────────┘
         │ N
         │
         │ 1
  ┌──────┴───────┐
  │   USUARIOS   │
  └──────────────┘