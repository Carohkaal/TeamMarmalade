for f in *.png; 
do mv "$f" "$(echo "$f" | tr -d '[:space:]' | tr '[:upper:]' '[:lower:]')";
done