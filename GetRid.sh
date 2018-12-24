OS="`uname`"
case $OS in
    'Linux') echo 'linux-x64';;
    'WindowsNT') echo 'win-x64';;
    'Darwin') echo 'osx-x64';;
esac
