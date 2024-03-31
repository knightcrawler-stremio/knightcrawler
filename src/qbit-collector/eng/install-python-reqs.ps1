Remove-Item -Recurse -Force ../python
mkdir -p ../python
python -m pip install -r ../requirements.txt -t ../python/