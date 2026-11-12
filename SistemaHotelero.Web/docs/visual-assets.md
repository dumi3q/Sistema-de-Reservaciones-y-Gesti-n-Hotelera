# Imágenes de HotelHub

Se utilizó la herramienta integrada de generación de imágenes de Codex.
Las imágenes son conceptos de un hotel ficticio, no fotografías del hotel real.
Se convirtieron a WebP para utilizarlas localmente en el sitio.

- `public/images/hotel-exterior.webp`: primera imagen del carrusel y portada pública.
- `public/images/hotel-garden.webp`: tercera imagen del carrusel.
- La segunda vista conserva la ilustración HTML/CSS original del panel HotelHub.

## Prompt: exterior

Create a refined editorial architectural photograph for the login carousel of HotelHub, a fictional boutique hotel. Portrait 4:5 composition, six-story cream limestone hotel with restrained forest-green shutters and bronze-gold details, tropical lush garden, tranquil entrance courtyard, soft warm morning light, understated luxury, realistic materials, muted sage green, ivory and champagne palette matching a modern green and cream hospitality dashboard. No people, no text, no watermark, no collage, no UI. Building centered with breathing room, suitable for cropping as a tall right-side login image. Save the generated image as a project asset.

## Prompt: jardín

Create an editorial photograph of a tranquil boutique hotel garden lounge and reflecting pool for HotelHub's login carousel. Portrait 4:5, elegant cream stone arches, olive and forest-green upholstered lounge chairs, brass lanterns, soft linen, palms and tropical greenery, peaceful afternoon light, quiet high-end hospitality. Restrained palette of forest green, sage, warm ivory and champagne gold; sophisticated and inviting, natural realistic texture. No people, no text, no watermark, no collage, no UI. Composition suitable for a tall right-side login panel.

## Navegación de la demostración

`/visitar` es la entrada pública. Habitaciones y menú no exigen cuenta.
Elegir una habitación o pedir desde el menú conduce a `/login`.
`/registro` valida el formulario sin crear cuentas ni almacenar contraseñas.
`/explorar/inicio` es un dashboard de demostración del personal, con datos ficticios.
No representa una sesión autenticada ni sustituye la autorización de la futura API.
La fotografía y las estadísticas no pertenecen a un hotel real.
