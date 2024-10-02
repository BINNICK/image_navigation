from flask import Flask, request, jsonify, render_template_string, send_from_directory, send_file
import requests
import json
import uuid
import time
import cv2
import numpy as np
import os

app = Flask(__name__)
app.config['UPLOAD_FOLDER'] = 'uploads/'
app.config['ALLOWED_EXTENSIONS'] = {'png', 'jpg', 'jpeg'}

# OCR API 설정
api_url = 'API'
secret_key = 'API'
# Google Maps Geocoding API 설정
google_api_key = 'API'

base_directory = "충북대학교 "

# 위에 언급된 파일의 형식이 맞는지 확인
def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in app.config['ALLOWED_EXTENSIONS']

# HTML, 자바스크립트 사용
@app.route('/')
def upload_form():
    html_content = '''
    <!DOCTYPE html>
    <html>
    <head>
        <title>Upload Image</title>
    </head>
    <body>
        <h1>Image Upload for OCR and Geocoding</h1>
        <form action="/upload" method="post" enctype="multipart/form-data">
            <input type="file" name="file" id="file" accept="image/png, image/jpeg" onchange="previewFile()">
            <input type="submit" value="Upload Image">
            <br><br>
            <img id="preview" src="#" alt="Image Preview..." style="width: 300px; display: none;">
            <script>
                function previewFile() {
                    var preview = document.getElementById('preview');
                    var file    = document.getElementById('file').files[0];
                    var reader  = new FileReader();

                    reader.onloadend = function () {
                        preview.src = reader.result;
                        preview.style.display = 'block';
                    }

                    if (file) {
                        reader.readAsDataURL(file);
                    } else {
                        preview.src = "";
                    }
                }
            </script>
        </form>
    </body>
    </html>
    '''
    return render_template_string(html_content)

# 파일을 업로드하는 함수
# 함수는 이미지를 업로드했을 때 호출
# 이미지를 서버에 저장하고, opencv를 사용해서 이미지를 읽는다.
@app.route('/upload', methods=['POST'])
def upload_image():
    if 'file' not in request.files:
        return jsonify({'error': 'No file part'})
    file = request.files['file']
    if file.filename == '' or not allowed_file(file.filename):
        return jsonify({'error': 'No selected file or invalid file format'})
    
    filename = uuid.uuid4().hex + '.' + file.filename.rsplit('.', 1)[1].lower()
    filepath = os.path.join(app.config['UPLOAD_FOLDER'], filename)
    file.save(filepath)

    npimg = np.fromfile(filepath, np.uint8)
    img = cv2.imdecode(npimg, cv2.IMREAD_COLOR)
    
    # 네이버 clova OCR
    files = {'file': ('image.jpg', open(filepath, 'rb'), 'image/jpeg')}
    request_json = {'images': [{'format': 'jpg', 'name': 'demo'}],
                    'requestId': str(uuid.uuid4()),
                    'version': 'V2',
                    'timestamp': int(round(time.time() * 1000))}
    payload = {'message': json.dumps(request_json).encode('UTF-8')}
    headers = {'X-OCR-SECRET': secret_key}
    
    response = requests.post(api_url, headers=headers, data=payload, files=files)
    ocr_result = response.json()
    

    # 구글 맵 
    if ocr_result and 'images' in ocr_result and len(ocr_result['images']) > 0 and 'fields' in ocr_result['images'][0]:
        for field in ocr_result['images'][0]['fields']:
            address = base_directory + field['inferText']
            geocode_url = f'https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={google_api_key}'
            geocode_response = requests.get(geocode_url)
            geocode_data = geocode_response.json()
            if 'results' in geocode_data and len(geocode_data['results']) > 0:
                location = geocode_data['results'][0]['geometry']['location']
                result_text = f"{location['lng']}, {location['lat']}"

                # 결과를 지정된 경로에 저장
                #result_filepath = r'C:\Users\bnlab\Desktop\template\extracted3_text.txt'
                result_filepath = r'C:\Users\82109\graduation\Assets\Resources\GPSCoordinates.txt'
                with open(result_filepath, 'w') as result_file:
                    result_file.write(result_text)
                
                return send_file(result_filepath, as_attachment=True, download_name='GPSCoordinates.txt')

    return jsonify({'error': 'No valid address found'})

# 애플리케이션을 시작할 때 필요한 디렉토리를 생성하고 Flask 서버를 시작한다.
if __name__ == '__main__':
    if not os.path.exists(app.config['UPLOAD_FOLDER']):
        os.makedirs(app.config['UPLOAD_FOLDER'])
    app.run(debug=True, host='0.0.0.0', port=8068)

 