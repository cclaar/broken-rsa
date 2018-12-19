<?php

function new_key() {
		$config = array(
	    //"digest_alg" => "sha512",
	    "private_key_bits" => 2048,
	    "private_key_type" => OPENSSL_KEYTYPE_RSA,
	);
	    
	// Create the private and public key
	$res = openssl_pkey_new($config);

	// Extract the private key from $res to $privKey
	openssl_pkey_export($res, $privKey);

	// Extract the public key from $res to $pubKey
	$pubKey = openssl_pkey_get_details($res);
	$pubKey = $pubKey["key"];

	file_put_contents($_SERVER["DOCUMENT_ROOT"].'location/key_gen.pub', $pubKey); 
	file_put_contents($_SERVER["DOCUMENT_ROOT"].'location/key_gen.prv', $privKey); 
}

function rsa_dec($data) {
	$res = openssl_pkey_get_private(file_get_contents($_SERVER["DOCUMENT_ROOT"].'location/key_gen.prv'));

	if($res) {
	    if(openssl_private_decrypt($data, $decrypted, $res)) {
	    	return $decrypted;
	    }
	}
    return false;
} 

function rsa_enc($data) {
	$pub_key = openssl_pkey_get_public(file_get_contents($_SERVER["DOCUMENT_ROOT"].'location/key_gen.pub')); 
	$keyData = openssl_pkey_get_details($pub_key);

	// Encrypt the data to $encrypted using the public key
	openssl_public_encrypt($data, $encrypted, $keyData['key']);

	return $encrypted;
} 

function Base64UrlDecode($x)
{
   return base64_decode(str_replace(array('_','-'), array('/','+'), $x));
}

function Base64UrlEncode($x)
{
   return str_replace(array('/','+'), array('_','-'), base64_encode($x));
}

echo "test";
echo $string = "I-6O9eY-Hl3-4C1FThfZqr67u_gbqlcX8kzptVsYvsQIZMNUbjAaVQzfcbbEIpjiU2PJj7_x64L-3GYW-bDtPHz4reeIbB8S_ItCAtNHFN5uR4RYt6nNqi_F25CErqdmj3kWXYnfCHXezrIT6PZGRwM45HTg2z1DgyyxqgdVN3cbqGAuSrebwVrLXe_Ch3YnkhMfwUeWODM-CPpa-_Y7U23s8HGAFzPjmmqDZwFqlxQE5Jt3y2C_1ba_dmOrnslgOsGMOsxnXsUBPjrLrRuw24ElkXvRcD877orNdN0GQ4-C4B19Pi7XaJzT02O27IMlvNzAh21kk8MHeZi2OmaHmKqXp3MXlmrIPztS61X3v1y0m5fjBOX9w02QSpZCxDmn67_nv-wW";
echo $data = Base64UrlDecode($string);
echo rsa_dec($data);

if(isset($_POST['data'])) {
	echo "hit";
	echo $_POST['data'];
	echo "<br>";

	echo $input = Base64UrlDecode($_POST['data']);
	//echo $hash = iconv('iso-2022-jp', 'UTF-8', $input);

	echo rsa_dec($input);
	//echo rsa_dec($hash);
}

?>